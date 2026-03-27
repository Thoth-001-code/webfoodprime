// utils/helpers.js
const Helpers = {
    formatCurrency: (amount) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount || 0);
    },
    
    formatDate: (dateString) => {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN') + ' ' + date.toLocaleTimeString('vi-VN');
    },
    
    formatDateTime: (dateString) => {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },
    
    getStatusText: (status) => {
        const map = {
            'Pending': '⏳ Chờ xác nhận',
            'Confirmed': '✅ Đã xác nhận',
            'Ready': '🍳 Sẵn sàng',
            'Delivering': '🚚 Đang giao',
            'Completed': '🎉 Hoàn thành',
            'Cancelled': '❌ Đã hủy'
        };
        return map[status] || status;
    },
    
    getStatusClass: (status) => {
        const map = {
            'Pending': 'status-pending',
            'Confirmed': 'status-confirmed',
            'Ready': 'status-ready',
            'Delivering': 'status-delivering',
            'Completed': 'status-completed',
            'Cancelled': 'status-cancelled'
        };
        return map[status] || '';
    },
    
    escapeHtml: (text) => {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    },
    
    debounce: (func, wait) => {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },
    
    getQueryParam: (param) => {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }
};

window.Helpers = Helpers;