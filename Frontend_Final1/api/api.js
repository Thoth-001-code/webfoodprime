// api/api.js
const API = {
    // ========== AUTH ==========
    register: (data) => axiosInstance.post('/Auth/register', data),
    login: (data) => axiosInstance.post('/Auth/login', data),
    getMe: () => axiosInstance.get('/Auth/me'),
    
    // ========== FOOD ==========
    getFoods: () => axiosInstance.get('/Food').catch(() => []),
    getFoodById: (id) => axiosInstance.get(`/Food/${id}`),
    getTopFoods: (top = 5) => axiosInstance.get(`/Admin/top-foods?top=${top}`).catch(() => []),
    createFood: (formData) => axiosInstance.post('/Food', formData, true),
    updateFood: (id, formData) => axiosInstance.put(`/Food/${id}`, formData, true),
    deleteFood: (id) => axiosInstance.delete(`/Food/${id}`),
    
    // ========== CART ==========
    getCart: () => axiosInstance.get('/Cart'),
    addToCart: (data) => axiosInstance.post('/Cart/add', data),
    updateCart: (data) => axiosInstance.put('/Cart/update', data),
    removeCartItem: (id) => axiosInstance.delete(`/Cart/${id}`),
    
    // ========== ADDRESS ==========
    getAddresses: () => axiosInstance.get('/Address'),
    createAddress: (data) => axiosInstance.post('/Address', data),
    updateAddress: (data) => axiosInstance.put('/Address', data),
    deleteAddress: (id) => axiosInstance.delete(`/Address/${id}`),
    
    // ========== ORDER ==========
    createOrder: (data) => {
        const paymentMethodValue = data.paymentMethod === 'Wallet' ? 1 : 2;
        return axiosInstance.post('/Order', {
            addressId: data.addressId,
            paymentMethod: paymentMethodValue,
            note: data.note || ''
        });
    },
    getMyOrders: () => axiosInstance.get('/Order/my'),
    getAllOrders: (status = '') => axiosInstance.get(`/Admin/orders?status=${status}`),
    confirmOrder: (orderId) => axiosInstance.post(`/Admin/confirm/${orderId}`),
    readyOrder: (orderId) => axiosInstance.post(`/Admin/ready/${orderId}`),
    cancelOrder: (orderId) => axiosInstance.post(`/Admin/cancel/${orderId}`),
    
    // ========== ADMIN ==========
    getDashboard: () => axiosInstance.get('/Admin/dashboard'),
    getPendingOrders: () => axiosInstance.get('/Admin/pending'),
    
    // ========== WALLET ==========
    getWallet: () => axiosInstance.get('/Wallet').catch(() => ({ balance: 0 })),
    getTransactions: () => axiosInstance.get('/Wallet/transactions').catch(() => []),
    deposit: (data) => axiosInstance.post('/Wallet/deposit', data),
    
    // ========== PAYMENT ==========
    payOrder: (orderId) => axiosInstance.post(`/Payment/pay/${orderId}`),
    
    // ========== SHIPPER ==========
    getAvailableOrders: () => axiosInstance.get('/Shipper/available-orders'),
    takeOrder: (orderId) => axiosInstance.post(`/Shipper/take/${orderId}`),
    updateShippingStatus: (data) => axiosInstance.put('/Shipper/status', data),
    getMyShippingOrders: () => axiosInstance.get('/Shipper/my-orders').catch(() => []),
};

window.API = API;