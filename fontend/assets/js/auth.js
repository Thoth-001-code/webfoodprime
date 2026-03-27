const Auth = {
    isAuthenticated: () => {
        return localStorage.getItem('token') !== null;
    },
    
    getCurrentUser: async () => {
        try {
            const user = await API.getMe();
            return user;
        } catch (error) {
            console.error('Get user error:', error);
            return null;
        }
    },
    
    getUserRole: async () => {
        try {
            const user = await Auth.getCurrentUser();
            if (user && user.roles && user.roles.length > 0) {
                return user.roles[0];
            }
            return null;
        } catch (error) {
            console.error('Get role error:', error);
            return null;
        }
    },
    
    login: async (email, password) => {
        try {
            const result = await API.login({ email, password });
            localStorage.setItem('token', result.token);
            const user = await Auth.getCurrentUser();
            Utils.showNotification('Đăng nhập thành công!', 'success');
            
            if (user && user.roles) {
                const role = user.roles[0];
                Auth.redirectByRole(role);
            } else {
                window.location.href = '/';
            }
            return { success: true, data: result };
        } catch (error) {
            Utils.showNotification(error.message, 'error');
            return { success: false, error: error.message };
        }
    },
    
    register: async (email, password, role = 'Customer') => {
        try {
            const result = await API.register({ email, password, role });
            Utils.showNotification('Đăng ký thành công! Vui lòng đăng nhập.', 'success');
            setTimeout(() => {
                window.location.href = '/pages/auth/login.html';
            }, 2000);
            return { success: true, data: result };
        } catch (error) {
            Utils.showNotification(error.message, 'error');
            return { success: false, error: error.message };
        }
    },
    
    logout: () => {
        localStorage.removeItem('token');
        Utils.showNotification('Đã đăng xuất', 'info');
        window.location.href = '/';
    },
    
    redirectByRole: (role) => {
        switch(role) {
            case 'Admin':
                window.location.href = '/pages/admin/dashboard.html';
                break;
            case 'Shipper':
                window.location.href = '/pages/shipper/available.html';
                break;
            case 'Customer':
                window.location.href = '/pages/user/menu.html';
                break;
            default:
                window.location.href = '/';
        }
    },
    
    updateNavigation: async () => {
        const navLinks = document.getElementById('navLinks');
        const userInfo = document.getElementById('userInfo');
        const ctaButtons = document.getElementById('ctaButtons');
        
        if (!navLinks) return;
        
        if (Auth.isAuthenticated()) {
            const user = await Auth.getCurrentUser();
            const role = user?.roles?.[0];
            
            let navHtml = '';
            if (role === 'Customer') {
                navHtml = `
                    <a href="/pages/user/menu.html">🍕 Thực đơn</a>
                    <a href="/pages/user/cart.html">🛒 Giỏ hàng</a>
                    <a href="/pages/user/orders.html">📦 Đơn hàng</a>
                    <a href="/pages/user/profile.html">👤 Tài khoản</a>
                `;
            } else if (role === 'Admin') {
                navHtml = `
                    <a href="/pages/admin/dashboard.html">📊 Dashboard</a>
                    <a href="/pages/admin/foods.html">🍕 Quản lý món ăn</a>
                    <a href="/pages/admin/orders.html">📋 Quản lý đơn hàng</a>
                `;
            } else if (role === 'Shipper') {
                navHtml = `
                    <a href="/pages/shipper/available.html">🚚 Đơn sẵn sàng</a>
                    <a href="/pages/shipper/delivering.html">📦 Đang giao</a>
                `;
            }
            
            navLinks.innerHTML = navHtml;
            
            userInfo.innerHTML = `
                <div class="user-dropdown">
                    <span class="user-email">${user?.email || 'User'}</span>
                    <button onclick="Auth.logout()" class="btn-logout">Đăng xuất</button>
                </div>
            `;
            
            if (ctaButtons) ctaButtons.style.display = 'none';
        } else {
            navLinks.innerHTML = `
                <a href="/">🏠 Trang chủ</a>
                <a href="/pages/user/menu.html">🍕 Thực đơn</a>
            `;
            
            userInfo.innerHTML = `
                <div class="auth-buttons">
                    <a href="/pages/auth/login.html" class="btn-login">Đăng nhập</a>
                    <a href="/pages/auth/register.html" class="btn-register">Đăng ký</a>
                </div>
            `;
        }
    },
    
    checkPageAccess: async (allowedRoles = []) => {
        if (allowedRoles.length === 0) return true;
        
        if (!Auth.isAuthenticated()) {
            Utils.showNotification('Vui lòng đăng nhập để tiếp tục', 'warning');
            window.location.href = '/pages/auth/login.html';
            return false;
        }
        
        const role = await Auth.getUserRole();
        
        if (!allowedRoles.includes(role)) {
            Utils.showNotification('Bạn không có quyền truy cập trang này', 'error');
            Auth.redirectByRole(role);
            return false;
        }
        
        return true;
    },
    
    init: async () => {
        await Auth.updateNavigation();
        
        const currentPath = window.location.pathname;
        const protectedPages = {
            '/pages/user/': ['Customer'],
            '/pages/admin/': ['Admin'],
            '/pages/shipper/': ['Shipper'],
            '/pages/user/cart.html': ['Customer'],
            '/pages/user/orders.html': ['Customer'],
            '/pages/user/profile.html': ['Customer'],
            '/pages/admin/dashboard.html': ['Admin'],
            '/pages/admin/foods.html': ['Admin'],
            '/pages/admin/orders.html': ['Admin'],
            '/pages/shipper/available.html': ['Shipper'],
            '/pages/shipper/delivering.html': ['Shipper']
        };
        
        for (const [path, roles] of Object.entries(protectedPages)) {
            if (currentPath.includes(path)) {
                await Auth.checkPageAccess(roles);
                break;
            }
        }
    }
};