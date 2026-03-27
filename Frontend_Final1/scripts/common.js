// scripts/common.js
const Common = {
    async initPage(allowedRoles = []) {
        // Kiểm tra đăng nhập
        if (allowedRoles.length > 0 && !Store.isAuthenticated()) {
            Toast.warning('Vui lòng đăng nhập để tiếp tục');
            setTimeout(() => {
                window.location.href = '/pages/auth/login.html';
            }, 1000);
            return false;
        }
        
        // Kiểm tra role
        if (allowedRoles.length > 0) {
            const user = await Store.loadUser();
            const role = user?.roles?.[0];
            
            if (!allowedRoles.includes(role)) {
                Toast.error('Bạn không có quyền truy cập trang này');
                setTimeout(() => {
                    window.location.href = '/';
                }, 1000);
                return false;
            }
        }
        
        // Cập nhật navbar
        await Navbar.update();
        return true;
    },
    
    getRoleFromPath() {
        const path = window.location.pathname;
        if (path.includes('/admin/')) return 'Admin';
        if (path.includes('/shipper/')) return 'Shipper';
        if (path.includes('/user/')) return 'Customer';
        return null;
    }
};

window.Common = Common;