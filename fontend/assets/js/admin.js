// Admin Module Functions
const Admin = {
    // ========== LOAD DASHBOARD ==========
    loadDashboard: async () => {
        try {
            console.log('📊 Loading dashboard...');
            const dashboard = await API.getDashboard();
            const topFoods = await API.getTopFoods(5);
            
            // Update stats
            const stats = {
                'total-revenue': dashboard.totalRevenue,
                'total-orders': dashboard.totalOrders,
                'pending-orders': dashboard.pendingOrders,
                'confirmed-orders': dashboard.confirmedOrders,
                'ready-orders': dashboard.readyOrders,
                'delivering-orders': dashboard.deliveringOrders,
                'completed-orders': dashboard.completedOrders,
                'cancelled-orders': dashboard.cancelledOrders
            };
            
            for (const [id, value] of Object.entries(stats)) {
                const elem = document.getElementById(id);
                if (elem) {
                    if (id === 'total-revenue') {
                        elem.textContent = Utils.formatCurrency(value);
                    } else {
                        elem.textContent = value;
                    }
                }
            }
            
            // Update top foods
            const topFoodsContainer = document.getElementById('top-foods');
            if (topFoodsContainer) {
                if (!topFoods || topFoods.length === 0) {
                    topFoodsContainer.innerHTML = '<p>Chưa có dữ liệu</p>';
                } else {
                    topFoodsContainer.innerHTML = topFoods.map((food, index) => `
                        <div class="top-food-item">
                            <div class="rank">${index + 1}</div>
                            <img src="${food.imageUrl || '/assets/images/food-placeholder.jpg'}" alt="${food.foodName}">
                            <div class="food-info">
                                <h4>${food.foodName}</h4>
                                <p>${Utils.formatCurrency(food.price)}</p>
                            </div>
                            <div class="sold-count">
                                <span class="label">Đã bán:</span>
                                <span class="count">${food.totalSold}</span>
                            </div>
                        </div>
                    `).join('');
                }
            }
        } catch (error) {
            console.error('Load dashboard error:', error);
            Utils.showNotification('Lỗi tải dashboard: ' + error.message, 'error');
        }
    },
    
    // ========== LOAD FOODS ==========
    loadFoods: async () => {
        try {
            const foods = await API.getFoods();
            const container = document.getElementById('foodsContainer');
            if (!container) return;
            
            if (!foods || foods.length === 0) {
                container.innerHTML = '<div class="empty-state">📭 Chưa có món ăn nào. Hãy thêm món ăn đầu tiên!</div>';
                return;
            }
            
            container.innerHTML = foods.map(food => `
                <div class="food-admin-card" data-id="${food.foodId}">
                    <div class="food-image">
                        ${food.imageUrl ? 
                            `<img src="${food.imageUrl}" alt="${food.foodName}" onerror="this.src='/assets/images/food-placeholder.jpg'">` :
                            `<div class="no-image">📷 Không có ảnh</div>`
                        }
                    </div>
                    <div class="food-details">
                        <h3>${Admin.escapeHtml(food.foodName)}</h3>
                        <p class="price">${Utils.formatCurrency(food.price)}</p>
                    </div>
                    <div class="food-actions">
                        <button onclick="Admin.editFood(${food.foodId})" class="btn-edit">✏️ Sửa</button>
                        <button onclick="Admin.deleteFood(${food.foodId})" class="btn-delete">🗑️ Xóa</button>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            Utils.showNotification('Lỗi tải món ăn: ' + error.message, 'error');
        }
    },
    
    // ========== CREATE FOOD ==========
    createFood: async () => {
        const name = document.getElementById('foodName').value.trim();
        const price = parseFloat(document.getElementById('foodPrice').value);
        const imageFile = document.getElementById('foodImage').files[0];
        
        if (!name) {
            Utils.showNotification('Vui lòng nhập tên món ăn', 'warning');
            return;
        }
        
        if (isNaN(price) || price <= 0) {
            Utils.showNotification('Vui lòng nhập giá hợp lệ', 'warning');
            return;
        }
        
        const formData = new FormData();
        formData.append('FoodName', name);
        formData.append('Price', price);
        if (imageFile) formData.append('Image', imageFile);
        
        const createBtn = document.querySelector('#addFoodForm .btn-submit');
        if (createBtn) {
            createBtn.disabled = true;
            createBtn.textContent = 'Đang xử lý...';
        }
        
        try {
            await API.createFood(formData);
            Utils.showNotification('✅ Thêm món ăn thành công!', 'success');
            Admin.hideAddFoodForm();
            await Admin.loadFoods();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        } finally {
            if (createBtn) {
                createBtn.disabled = false;
                createBtn.textContent = 'Thêm món ăn';
            }
        }
    },
    
    // ========== EDIT FOOD ==========
    editFood: async (foodId) => {
        try {
            const foods = await API.getFoods();
            const food = foods.find(f => f.foodId === foodId);
            
            if (!food) {
                Utils.showNotification('Không tìm thấy món ăn', 'error');
                return;
            }
            
            document.getElementById('addFoodForm').style.display = 'none';
            document.getElementById('editFoodForm').style.display = 'block';
            document.getElementById('editFoodId').value = food.foodId;
            document.getElementById('editFoodName').value = food.foodName;
            document.getElementById('editFoodPrice').value = food.price;
            document.getElementById('editFoodImage').value = '';
            
            const currentImage = document.getElementById('currentImage');
            if (currentImage) {
                if (food.imageUrl) {
                    currentImage.innerHTML = `
                        <p>📷 Ảnh hiện tại:</p>
                        <img src="${food.imageUrl}" alt="${food.foodName}" style="max-width: 150px;">
                    `;
                    currentImage.style.display = 'block';
                } else {
                    currentImage.innerHTML = '<p>📷 Không có ảnh</p>';
                    currentImage.style.display = 'block';
                }
            }
        } catch (error) {
            Utils.showNotification('Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== UPDATE FOOD ==========
    updateFood: async () => {
        const foodId = parseInt(document.getElementById('editFoodId').value);
        const name = document.getElementById('editFoodName').value.trim();
        const price = parseFloat(document.getElementById('editFoodPrice').value);
        const imageFile = document.getElementById('editFoodImage').files[0];
        
        if (!name || isNaN(price) || price <= 0) {
            Utils.showNotification('Vui lòng nhập đầy đủ thông tin', 'warning');
            return;
        }
        
        const formData = new FormData();
        formData.append('FoodName', name);
        formData.append('Price', price);
        if (imageFile) formData.append('Image', imageFile);
        
        try {
            await API.updateFood(foodId, formData);
            Utils.showNotification('✅ Cập nhật thành công!', 'success');
            Admin.hideEditFoodForm();
            await Admin.loadFoods();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== DELETE FOOD ==========
    deleteFood: async (foodId) => {
        if (!confirm('Bạn có chắc muốn xóa món ăn này?')) return;
        
        try {
            await API.deleteFood(foodId);
            Utils.showNotification('✅ Xóa thành công!', 'success');
            await Admin.loadFoods();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== LOAD ADMIN ORDERS ==========
    loadAdminOrders: async (status = '') => {
        try {
            const orders = await API.getAllOrders(status);
            const container = document.getElementById('ordersContainer');
            const statusFilter = document.getElementById('statusFilter');
            
            if (statusFilter) statusFilter.value = status;
            if (!container) return;
            
            if (!orders || orders.length === 0) {
                container.innerHTML = '<div class="empty-state">Không có đơn hàng nào</div>';
                return;
            }
            
            container.innerHTML = orders.map(order => `
                <div class="order-admin-card">
                    <div class="order-header">
                        <div>
                            <h4>Đơn hàng #${order.orderId}</h4>
                            <p class="order-date">${Utils.formatDate(order.createdAt)}</p>
                        </div>
                        <div class="order-badges">
                            <span class="status ${order.status.toLowerCase()}">${Admin.getStatusText(order.status)}</span>
                            <span class="order-type">${order.orderType === 'Delivery' ? '🚚 Giao hàng' : '🏪 Tại quầy'}</span>
                        </div>
                    </div>
                    <div class="order-info">
                        <div class="info-row"><span class="label">Khách hàng:</span><span>${order.userId || 'Khách lẻ'}</span></div>
                        ${order.address ? `<div class="info-row"><span class="label">Địa chỉ:</span><span>${order.address}</span></div>` : ''}
                        <div class="info-row"><span class="label">Thanh toán:</span><span>${order.paymentMethod} ${order.isPaid ? '✅' : '⏳'}</span></div>
                    </div>
                    <div class="order-items">
                        <h5>Chi tiết:</h5>
                        ${order.items.map(item => `
                            <div class="order-item"><span>${item.foodName} x ${item.quantity}</span><span>${Utils.formatCurrency(item.price * item.quantity)}</span></div>
                        `).join('')}
                    </div>
                    <div class="order-footer">
                        <div class="order-total">
                            <span>Tạm tính: ${Utils.formatCurrency(order.foodTotal)}</span>
                            <span>Phí ship: ${Utils.formatCurrency(order.shippingFee)}</span>
                            <span class="total">Tổng: ${Utils.formatCurrency(order.totalPrice)}</span>
                        </div>
                        <div class="order-actions">
                            ${order.status === 'Pending' ? `
                                <button onclick="Admin.confirmOrder(${order.orderId})" class="btn-confirm">✓ Xác nhận</button>
                                <button onclick="Admin.cancelOrder(${order.orderId})" class="btn-cancel">✗ Hủy</button>
                            ` : ''}
                            ${order.status === 'Confirmed' ? `
                                <button onclick="Admin.readyOrder(${order.orderId})" class="btn-ready">✓ Chuẩn bị xong</button>
                            ` : ''}
                        </div>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            Utils.showNotification('Lỗi tải đơn hàng: ' + error.message, 'error');
        }
    },
    
    // ========== CONFIRM ORDER ==========
    confirmOrder: async (orderId) => {
        try {
            await API.confirmOrder(orderId);
            Utils.showNotification('✅ Xác nhận đơn hàng thành công!', 'success');
            Admin.filterOrders();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== READY ORDER ==========
    readyOrder: async (orderId) => {
        try {
            await API.readyOrder(orderId);
            Utils.showNotification('✅ Đơn hàng đã sẵn sàng!', 'success');
            Admin.filterOrders();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== CANCEL ORDER ==========
    cancelOrder: async (orderId) => {
        if (!confirm('Bạn có chắc muốn hủy đơn hàng này?')) return;
        try {
            await API.cancelOrder(orderId);
            Utils.showNotification('✅ Hủy đơn thành công!', 'success');
            Admin.filterOrders();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== FILTER ORDERS ==========
    filterOrders: () => {
        const status = document.getElementById('statusFilter').value;
        Admin.loadAdminOrders(status);
    },
    
    // ========== GET STATUS TEXT ==========
    getStatusText: (status) => {
        const map = { 'Pending': 'Chờ xác nhận', 'Confirmed': 'Đã xác nhận', 'Ready': 'Sẵn sàng', 'Delivering': 'Đang giao', 'Completed': 'Hoàn thành', 'Cancelled': 'Đã hủy' };
        return map[status] || status;
    },
    
    // ========== HIDE FORMS ==========
    hideAddFoodForm: () => {
        document.getElementById('addFoodForm').style.display = 'none';
        document.getElementById('foodName').value = '';
        document.getElementById('foodPrice').value = '';
        document.getElementById('foodImage').value = '';
    },
    
    hideEditFoodForm: () => {
        document.getElementById('editFoodForm').style.display = 'none';
        document.getElementById('editFoodName').value = '';
        document.getElementById('editFoodPrice').value = '';
        document.getElementById('editFoodImage').value = '';
    },
    
    showAddFoodForm: () => {
        document.getElementById('addFoodForm').style.display = 'block';
        document.getElementById('editFoodForm').style.display = 'none';
    },
    
    escapeHtml: (text) => {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
};