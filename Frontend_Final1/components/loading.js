// components/loading.js
const Loading = {
    show() {
        let loader = document.getElementById('loadingOverlay');
        if (!loader) {
            loader = document.createElement('div');
            loader.id = 'loadingOverlay';
            loader.className = 'loading-overlay';
            loader.innerHTML = '<div class="loading-spinner"></div><p>Đang tải...</p>';
            document.body.appendChild(loader);
        }
        loader.style.display = 'flex';
    },
    
    hide() {
        const loader = document.getElementById('loadingOverlay');
        if (loader) {
            loader.style.display = 'none';
        }
    },
    
    async wrap(fn) {
        this.show();
        try {
            return await fn();
        } finally {
            this.hide();
        }
    }
};

window.Loading = Loading;