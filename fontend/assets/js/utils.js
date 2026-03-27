// Utility functions
const Utils = {
    showNotification: (message, type = 'success') => {
        const existing = document.querySelector('.notification');
        if (existing) existing.remove();
        
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <span class="notification-icon">
                ${type === 'success' ? '✓' : type === 'error' ? '✗' : 'ℹ'}
            </span>
            <span class="notification-message">${message}</span>
        `;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.classList.add('fade-out');
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    },
    
    formatCurrency: (amount) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    },
    
    formatDate: (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN') + ' ' + date.toLocaleTimeString('vi-VN');
    },
    
    showLoading: () => {
        const loader = document.createElement('div');
        loader.id = 'loadingSpinner';
        loader.className = 'loading-spinner';
        loader.innerHTML = '<div class="spinner"></div>';
        document.body.appendChild(loader);
    },
    
    hideLoading: () => {
        const loader = document.getElementById('loadingSpinner');
        if (loader) loader.remove();
    },
    
    validateEmail: (email) => {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },
    
    validatePasswordStrength: (password) => {
        const hasUppercase = /[A-Z]/.test(password);
        const hasLowercase = /[a-z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[!@#$%^&*]/.test(password);
        const hasMinLength = password.length >= 6;
        
        return {
            isValid: hasUppercase && hasLowercase && hasNumber && hasSpecial && hasMinLength,
            hasUppercase,
            hasLowercase,
            hasNumber,
            hasSpecial,
            hasMinLength
        };
    },
    
    getQueryParam: (param) => {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    },
    
    debug: (...args) => {
        if (localStorage.getItem('debug') === 'true') {
            console.log('[DEBUG]', ...args);
        }
    },
    
    enableDebug: () => {
        localStorage.setItem('debug', 'true');
        console.log('Debug mode enabled');
    },
    
    disableDebug: () => {
        localStorage.setItem('debug', 'false');
        console.log('Debug mode disabled');
    }
};