// components/modal.js
const Modal = {
    show(options) {
        const { title, content, onConfirm, onCancel, confirmText = 'Xác nhận', cancelText = 'Hủy' } = options;
        
        const existing = document.querySelector('.modal');
        if (existing) existing.remove();
        
        const modal = document.createElement('div');
        modal.className = 'modal';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3>${title}</h3>
                    <button class="modal-close">&times;</button>
                </div>
                <div class="modal-body">
                    ${content}
                </div>
                <div class="modal-footer">
                    <button class="btn-modal-cancel">${cancelText}</button>
                    <button class="btn-modal-confirm">${confirmText}</button>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        
        const close = () => modal.remove();
        
        modal.querySelector('.modal-close').onclick = () => {
            close();
            if (onCancel) onCancel();
        };
        
        modal.querySelector('.btn-modal-cancel').onclick = () => {
            close();
            if (onCancel) onCancel();
        };
        
        modal.querySelector('.btn-modal-confirm').onclick = () => {
            close();
            if (onConfirm) onConfirm();
        };
        
        modal.onclick = (e) => {
            if (e.target === modal) {
                close();
                if (onCancel) onCancel();
            }
        };
    },
    
    confirm(title, message, onConfirm) {
        this.show({
            title,
            content: `<p>${message}</p>`,
            onConfirm,
            confirmText: 'Đồng ý',
            cancelText: 'Hủy'
        });
    }
};

window.Modal = Modal;