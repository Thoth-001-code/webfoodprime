// ============================================
// SHIPPER MODULE - FIXED
// ============================================

const Shipper = {
    // Load dashboard
    loadDashboard: async () => {
        try {
            // Get available orders count
            let availableCount = 0;
            try {
                const availableOrders = await API.getAvailableOrders();
                availableCount = availableOrders?.length || 0;
            } catch (e) {
                console.log('Cannot load available orders:', e);
            }
            
            // Get my orders
            let deliveringCount = 0;
            let completedCount = 0;
            let totalEarned = 0;
            
            try {
                const myOrders = await API.getMyShippingOrders();
                if (myOrders && myOrders.length) {
                    deliveringCount = myOrders.filter(o => o.status === 'Delivering').length;
                    const completedOrders = myOrders.filter(o => o.status === 'Completed');
                    completedCount = completedOrders.length;
                    totalEarned = completedOrders.reduce((sum, o) => sum + (o.shippingFee || 15000), 0);
                }
            } catch (e) {
                console.log('Cannot load my orders:', e);
            }
            
            // Update stats
            const availableElem = document.getElementById('available-orders');
            const deliveringElem = document.getElementById('my-delivering');
            const completedElem = document.getElementById('my-completed');
            const earnedElem = document.getElementById('total-earned');
            
            if (availableElem) availableElem.textContent = availableCount;
            if (deliveringElem) deliveringElem.textContent = deliveringCount;
            if (completedElem) completedElem.textContent = completedCount;
            if (earnedElem) earnedElem.textContent = Utils.formatCurrency(totalEarned);
            
            // Load recent deliveries
            const recentContainer = document.getElementById('recent-deliveries');
            if (recentContainer) {
                try {
                    const myOrders = await API.getMyShippingOrders('Completed');
                    const recentOrders = (myOrders || []).slice(0, 5);
                    
                    if (recentOrders.length === 0) {
                        recentContainer.innerHTML = '<p>Chưa có đơn hàng nào</p>';
                    } else {
                        recentContainer.innerHTML = recentOrders.map(order => `
                            <div class="recent-order-item">
                                <div class="order-info">
                                    <span class="order-id">#${order.orderId}</span>
                                    <span class="order-address">${order.address || 'Địa chỉ không xác định'}</span>
                                </div>
                                <div class="order-status completed">✅ Hoàn thành</div>
                            </div>
                        `).join('');
                    }
                } catch (e) {
                    recentContainer.innerHTML = '<p>Không thể tải dữ liệu</p>';
                }
            }
        } catch (error) {
            console.error('Load dashboard error:', error);
        }
    },
    
    // Load available orders
    loadAvailableOrders: async () => {
        try {
            const orders = await API.getAvailableOrders();
            const container = document.getElementById('availableOrdersContainer');
            
            if (!container) return;
            
            if (!orders || orders.length === 0) {
                container.innerHTML = `
                    <div class="empty-state">
                        🚚 Không có đơn hàng nào sẵn sàng
                        <p class="empty-sub">Hãy chờ Admin xác nhận đơn hàng</p>
                        <button onclick="Shipper.loadAvailableOrders()" class="refresh-btn" style="margin-top: 1rem;">
                            🔄 Làm mới
                        </button>
                    </div>
                `;
                return;
            }
            
            container.innerHTML = orders.map(order => `
                <div class="order-card available-order">
                    <div class="order-header">
                        <div>
                            <span class="badge">📦 Đơn #${order.orderId}</span>
                            <span class="order-time">${Utils.formatDate(order.createdAt)}</span>
                        </div>
                        <div class="payment-badge ${order.paymentMethod === 'Wallet' ? 'wallet' : 'cod'}">
                            ${order.paymentMethod === 'Wallet' ? '💳 Ví' : '💰 COD'}
                        </div>
                    </div>
                    <div class="order-details">
                        <div class="info-row">
                            <span class="label">📍 Địa chỉ:</span>
                            <span class="value">${order.address || 'Không có'}</span>
                        </div>
                        <div class="info-row">
                            <span class="label">📝 Ghi chú:</span>
                            <span class="value">${order.note || 'Không'}</span>
                        </div>
                    </div>
                    <div class="order-items">
                        <h4>🛒 Chi tiết:</h4>
                        ${order.items?.map(item => `
                            <div class="item-row">
                                <span>${Shipper.escapeHtml(item.foodName)} x ${item.quantity}</span>
                                <span>${Utils.formatCurrency(item.price * item.quantity)}</span>
                            </div>
                        `).join('') || '<p>Không có sản phẩm</p>'}
                    </div>
                    <div class="order-footer">
                        <div class="order-total">
                            <span>Tổng: <strong>${Utils.formatCurrency(order.totalPrice)}</strong></span>
                            <span>Phí ship: <strong>${Utils.formatCurrency(order.shippingFee || 15000)}</strong></span>
                        </div>
                        <button onclick="Shipper.takeOrder(${order.orderId})" class="btn-take-order">
                            🚚 Nhận đơn
                        </button>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            console.error('Load available orders error:', error);
            const container = document.getElementById('availableOrdersContainer');
            if (container) {
                container.innerHTML = `<div class="empty-state">❌ Lỗi: ${error.message}<button onclick="Shipper.loadAvailableOrders()" class="refresh-btn">🔄 Thử lại</button></div>`;
            }
        }
    },
    
    // Take order
    takeOrder: async (orderId) => {
        if (!confirm('Nhận giao đơn hàng này?')) return;
        
        Utils.showLoading();
        try {
            await API.takeOrder(orderId);
            Utils.hideLoading();
            Utils.showNotification('✅ Nhận đơn thành công!', 'success');
            
            await Shipper.loadAvailableOrders();
            
            setTimeout(async () => {
                const goToDelivering = confirm('Đã nhận đơn! Bạn có muốn xem đơn đang giao không?');
                if (goToDelivering) {
                    window.location.href = '/pages/shipper/delivering.html';
                }
            }, 500);
        } catch (error) {
            Utils.hideLoading();
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // Load delivering orders
    loadDeliveringOrders: async () => {
        try {
            const myOrders = await API.getMyShippingOrders();
            const deliveringOrders = (myOrders || []).filter(o => o.status === 'Delivering');
            const container = document.getElementById('deliveringOrdersContainer');
            
            if (!container) return;
            
            if (!deliveringOrders || deliveringOrders.length === 0) {
                container.innerHTML = `
                    <div class="empty-state">
                        🚚 Bạn không có đơn hàng nào đang giao
                        <a href="/pages/shipper/available.html" class="btn-view-available">📦 Xem đơn sẵn sàng</a>
                        <button onclick="Shipper.loadDeliveringOrders()" class="refresh-btn" style="margin-top: 1rem;">🔄 Làm mới</button>
                    </div>
                `;
                return;
            }
            
            container.innerHTML = deliveringOrders.map(order => `
                <div class="order-card delivering-order">
                    <div class="order-header">
                        <div>
                            <span class="badge delivering">🚚 Đang giao - #${order.orderId}</span>
                            <span class="order-time">${Utils.formatDate(order.createdAt)}</span>
                        </div>
                        <div class="status-badge delivering">🚚 Đang giao</div>
                    </div>
                    <div class="order-details">
                        <div class="info-row">
                            <span class="label">📍 Địa chỉ:</span>
                            <span class="value">${order.address || 'Không có'}</span>
                        </div>
                        <div class="info-row">
                            <span class="label">📝 Ghi chú:</span>
                            <span class="value">${order.note || 'Không'}</span>
                        </div>
                    </div>
                    <div class="order-items">
                        <h4>🛒 Chi tiết:</h4>
                        ${order.items?.map(item => `
                            <div class="item-row">
                                <span>${Shipper.escapeHtml(item.foodName)} x ${item.quantity}</span>
                                <span>${Utils.formatCurrency(item.price * item.quantity)}</span>
                            </div>
                        `).join('') || '<p>Không có sản phẩm</p>'}
                    </div>
                    <div class="order-footer">
                        <div class="order-total">
                            <span>Tổng: <strong>${Utils.formatCurrency(order.totalPrice)}</strong></span>
                            <span>Phí ship: <strong>${Utils.formatCurrency(order.shippingFee || 15000)}</strong></span>
                            <span>TT: <strong>${order.paymentMethod === 'Wallet' ? 'Ví' : 'COD'}</strong></span>
                        </div>
                        <button onclick="Shipper.completeOrder(${order.orderId})" class="btn-complete-order">
                            ✅ Hoàn thành giao
                        </button>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            console.error('Load delivering orders error:', error);
            const container = document.getElementById('deliveringOrdersContainer');
            if (container) {
                container.innerHTML = `<div class="empty-state">❌ Lỗi: ${error.message}<button onclick="Shipper.loadDeliveringOrders()" class="refresh-btn">🔄 Thử lại</button></div>`;
            }
        }
    },
    
    // Complete order
    completeOrder: async (orderId) => {
        if (!confirm('Xác nhận đã giao hàng thành công?')) return;
        
        Utils.showLoading();
        try {
            await API.updateShippingStatus({ orderId: orderId, status: 'Completed' });
            Utils.hideLoading();
            Utils.showNotification('✅ Giao hàng thành công!', 'success');
            
            await Shipper.loadDeliveringOrders();
            
            setTimeout(async () => {
                const goToAvailable = confirm('Giao hàng thành công! Bạn có muốn xem đơn sẵn sàng không?');
                if (goToAvailable) {
                    window.location.href = '/pages/shipper/available.html';
                }
            }, 500);
        } catch (error) {
            Utils.hideLoading();
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    escapeHtml: (text) => {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
};

window.Shipper = Shipper;