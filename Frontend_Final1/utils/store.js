// utils/store.js
const Store = {
    state: {
        user: null,
        token: localStorage.getItem('token'),
        cart: [],
    },
    
    get token() {
        return this.state.token;
    },
    
    set token(value) {
        this.state.token = value;
        if (value) {
            localStorage.setItem('token', value);
        } else {
            localStorage.removeItem('token');
        }
    },
    
    get user() {
        return this.state.user;
    },
    
    set user(value) {
        this.state.user = value;
    },
    
    get cart() {
        const saved = localStorage.getItem('cart');
        return saved ? JSON.parse(saved) : [];
    },
    
    set cart(value) {
        this.state.cart = value;
        localStorage.setItem('cart', JSON.stringify(value));
    },
    
    addToCart(item) {
        const cart = this.cart;
        const existing = cart.find(i => i.foodId === item.foodId);
        if (existing) {
            existing.quantity += item.quantity;
        } else {
            cart.push(item);
        }
        this.cart = cart;
    },
    
    removeFromCart(foodId) {
        this.cart = this.cart.filter(i => i.foodId !== foodId);
    },
    
    updateQuantity(foodId, quantity) {
        const cart = this.cart;
        const item = cart.find(i => i.foodId === foodId);
        if (item) {
            if (quantity <= 0) {
                this.removeFromCart(foodId);
            } else {
                item.quantity = quantity;
                this.cart = cart;
            }
        }
    },
    
    clearCart() {
        this.cart = [];
    },
    
    getCartTotal() {
        return this.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    },
    
    getCartCount() {
        return this.cart.reduce((sum, item) => sum + item.quantity, 0);
    },
    
    isAuthenticated() {
        return !!this.token;
    },
    
    async loadUser() {
        if (!this.token) return null;
        try {
            this.user = await API.getMe();
            return this.user;
        } catch (error) {
            console.error('Load user error:', error);
            this.token = null;
            return null;
        }
    },
    
    logout() {
        this.token = null;
        this.user = null;
        this.clearCart();
    }
};

window.Store = Store;