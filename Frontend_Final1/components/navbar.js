// components/navbar.js
const Navbar = {
    async update() {
        const navLinks = document.getElementById('navLinks');
        const userInfo = document.getElementById('userInfo');
        
        if (!navLinks) return;
        
        if (Store.isAuthenticated()) {
            const user = await Store.loadUser();
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
                    <button onclick="Navbar.logout()" class="btn-logout">Đăng xuất</button>
                </div>
            `;
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
    
    async logout() {
        Modal.confirm('Đăng xuất', 'Bạn có chắc muốn đăng xuất?', () => {
            Store.logout();
            Toast.success('Đã đăng xuất');
            window.location.href = '/';
        });
    }
};

window.Navbar = Navbar;