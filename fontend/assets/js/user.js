// ============================================
// USER MODULE - COMPLETE VERSION
// ============================================

const User = {
    // ========== LOAD WALLET ==========
    loadWallet: async () => {
        try {
            const wallet = await API.getWallet();
            const balanceElem = document.getElementById('walletBalance');
            if (balanceElem) {
                balanceElem.textContent = Utils.formatCurrency(wallet.balance);
            }
            
            const transactions = await API.getTransactions();
            const transactionsList = document.getElementById('transactionsList');
            if (transactionsList && transactions && transactions.length > 0) {
                transactionsList.innerHTML = transactions.map(t => {
                    const isDeposit = t.type === 'Deposit';
                    const sign = isDeposit ? '+' : '-';
                    const colorClass = isDeposit ? 'deposit' : 'payment';
                    return `
                        <div class="transaction-item">
                            <div class="transaction-info">
                                <div class="transaction-description">${t.description || (isDeposit ? 'Nạp tiền' : 'Thanh toán')}</div>
                                <div class="transaction-date">${Utils.formatDate(t.createdAt)}</div>
                            </div>
                            <div class="transaction-amount ${colorClass}">
                                ${sign} ${Utils.formatCurrency(t.amount)}
                            </div>
                        </div>
                    `;
                }).join('');
            } else if (transactionsList) {
                transactionsList.innerHTML = '<div class="empty-transactions">📭 Chưa có giao dịch nào</div>';
            }
        } catch (error) {
            console.error('Load wallet error:', error);
        }
    },
    
    // ========== DEPOSIT ==========
    deposit: async () => {
        const amount = parseFloat(document.getElementById('depositAmount')?.value);
        if (isNaN(amount) || amount <= 0) {
            Utils.showNotification('Vui lòng nhập số tiền hợp lệ', 'warning');
            return;
        }
        
        const depositBtn = document.querySelector('.btn-deposit');
        if (depositBtn) {
            depositBtn.disabled = true;
            depositBtn.textContent = 'Đang xử lý...';
        }
        
        try {
            await API.deposit({ amount });
            Utils.showNotification(`✅ Nạp tiền thành công! +${Utils.formatCurrency(amount)}`, 'success');
            document.getElementById('depositAmount').value = '';
            await User.loadWallet();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        } finally {
            if (depositBtn) {
                depositBtn.disabled = false;
                depositBtn.textContent = 'Nạp tiền';
            }
        }
    },
    
    // ========== LOAD ADDRESSES ==========
    loadAddresses: async () => {
        try {
            const addresses = await API.getAddresses();
            const container = document.getElementById('addressesList');
            if (!container) return;
            
            if (!addresses || addresses.length === 0) {
                container.innerHTML = '<div class="empty-transactions">📭 Chưa có địa chỉ nào</div>';
                return;
            }
            
            container.innerHTML = addresses.map(addr => `
                <div class="address-item">
                    <div class="address-info">
                        <p><strong>🏠 Địa chỉ:</strong> ${addr.fullAddress}</p>
                        ${addr.note ? `<p><strong>📝 Ghi chú:</strong> ${addr.note}</p>` : ''}
                    </div>
                    <button onclick="User.deleteAddress(${addr.addressId})" class="btn-delete">Xóa</button>
                </div>
            `).join('');
        } catch (error) {
            Utils.showNotification('Lỗi tải địa chỉ: ' + error.message, 'error');
        }
    },
    
    // ========== DELETE ADDRESS ==========
    deleteAddress: async (addressId) => {
        if (!confirm('Bạn có chắc muốn xóa địa chỉ này?')) return;
        try {
            await API.deleteAddress(addressId);
            Utils.showNotification('✅ Xóa địa chỉ thành công', 'success');
            await User.loadAddresses();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== ADD NEW ADDRESS FROM PROFILE ==========
    addNewAddressFromProfile: async () => {
        const fullAddress = document.getElementById('newAddressProfile')?.value.trim();
        const note = document.getElementById('addressNoteProfile')?.value.trim();
        
        if (!fullAddress) {
            Utils.showNotification('Vui lòng nhập địa chỉ', 'warning');
            return;
        }
        
        try {
            await API.createAddress({ fullAddress, note });
            Utils.showNotification('✅ Thêm địa chỉ thành công', 'success');
            document.getElementById('newAddressProfile').value = '';
            document.getElementById('addressNoteProfile').value = '';
            document.getElementById('addAddressForm').style.display = 'none';
            await User.loadAddresses();
        } catch (error) {
            Utils.showNotification('❌ Lỗi: ' + error.message, 'error');
        }
    },
    
    // ========== SHOW ADD ADDRESS FORM ==========
    showAddAddressForm: () => {
        const form = document.getElementById('addAddressForm');
        if (form) {
            form.style.display = form.style.display === 'none' ? 'block' : 'none';
        }
    },
    
    // ========== SHOW TAB ==========
    showTab: (tab, event) => {
        document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
        if (event && event.target) event.target.classList.add('active');
        
        const walletTab = document.getElementById('walletTab');
        const addressesTab = document.getElementById('addressesTab');
        
        if (walletTab) walletTab.classList.remove('active');
        if (addressesTab) addressesTab.classList.remove('active');
        
        if (tab === 'wallet') {
            if (walletTab) walletTab.classList.add('active');
        } else {
            if (addressesTab) addressesTab.classList.add('active');
        }
    },
    
    // ========== ESCAPE HTML ==========
    escapeHtml: (text) => {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
};

// Export for global use
window.User = User;