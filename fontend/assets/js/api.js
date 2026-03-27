// ============================================
// API CONFIGURATION
// ============================================
const API_BASE_URL = 'https://localhost:7229/api';

// ============================================
// HELPER FUNCTION
// ============================================
async function apiCall(endpoint, method = 'GET', data = null, isFormData = false) {
    const token = localStorage.getItem('token');
    const headers = {};
    
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    
    if (!isFormData) {
        headers['Content-Type'] = 'application/json';
    }
    
    const config = {
        method,
        headers,
    };
    
    if (data) {
        if (isFormData) {
            config.body = data;
        } else {
            config.body = JSON.stringify(data);
        }
    }
    
    try {
        console.log(`📡 API: ${method} ${API_BASE_URL}${endpoint}`);
        
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        const text = await response.text();
        
        console.log(`📥 Response status: ${response.status}`);
        if (text.length < 500) {
            console.log(`📥 Response:`, text);
        } else {
            console.log(`📥 Response:`, text.substring(0, 500) + '...');
        }
        
        let result;
        try {
            result = JSON.parse(text);
        } catch (e) {
            result = text;
        }
        
        if (!response.ok) {
            let errorMessage = '';
            if (typeof result === 'object') {
                if (result.message) errorMessage = result.message;
                else if (result.errors) errorMessage = Object.values(result.errors).flat().join(', ');
                else if (result.title) errorMessage = result.title;
                else errorMessage = JSON.stringify(result);
            } else {
                errorMessage = result || `HTTP Error ${response.status}`;
            }
            throw new Error(errorMessage);
        }
        
        return result;
    } catch (error) {
        console.error('❌ API Error:', error);
        throw error;
    }
}

// ============================================
// API METHODS
// ============================================
const API = {
    // ========== AUTH ==========
    register: (data) => apiCall('/Auth/register', 'POST', data),
    login: (data) => apiCall('/Auth/login', 'POST', data),
    getMe: () => apiCall('/Auth/me', 'GET'),
    
    // ========== FOOD ==========
    getFoods: async () => {
        try {
            const foods = await apiCall('/Food', 'GET');
            return foods || [];
        } catch (error) {
            console.error('Error fetching foods:', error);
            return [];
        }
    },
    getFoodById: (id) => apiCall(`/Food/${id}`, 'GET'),
    getTopFoods: async (top = 5) => {
        try {
            return await apiCall(`/Admin/top-foods?top=${top}`, 'GET');
        } catch (error) {
            return [];
        }
    },
    createFood: (formData) => apiCall('/Food', 'POST', formData, true),
    updateFood: (id, formData) => {
        if (!id || isNaN(id)) return Promise.reject(new Error('ID món ăn không hợp lệ'));
        return apiCall(`/Food/${id}`, 'PUT', formData, true);
    },
    deleteFood: (id) => {
        if (!id || isNaN(id)) return Promise.reject(new Error('ID món ăn không hợp lệ'));
        return apiCall(`/Food/${id}`, 'DELETE');
    },
    
    // ========== CART ==========
    getCart: () => apiCall('/Cart', 'GET'),
    addToCart: (data) => apiCall('/Cart/add', 'POST', data),
    updateCart: (data) => apiCall('/Cart/update', 'PUT', data),
    removeCartItem: (id) => apiCall(`/Cart/${id}`, 'DELETE'),
    
    // ========== ADDRESS ==========
    getAddresses: () => apiCall('/Address', 'GET'),
    createAddress: (data) => apiCall('/Address', 'POST', data),
    updateAddress: (data) => apiCall('/Address', 'PUT', data),
    deleteAddress: (id) => apiCall(`/Address/${id}`, 'DELETE'),
    
    // ========== ORDER ==========
    createOrder: async (data) => {
        console.log('📦 Creating order with data:', data);
        
        // Convert payment method to enum value (Wallet=1, COD=2)
        const paymentMethodValue = data.paymentMethod === 'Wallet' ? 1 : 2;
        
        const orderData = {
            addressId: data.addressId,
            paymentMethod: paymentMethodValue,
            note: data.note || ''
        };
        
        console.log('Sending order data:', orderData);
        return apiCall('/Order', 'POST', orderData);
    },
    getMyOrders: () => apiCall('/Order/my', 'GET'),
    getPendingOrders: () => apiCall('/Admin/pending', 'GET'),
    confirmOrder: (orderId) => apiCall(`/Admin/confirm/${orderId}`, 'POST'),
    readyOrder: (orderId) => apiCall(`/Admin/ready/${orderId}`, 'POST'),
    cancelOrder: (orderId) => apiCall(`/Admin/cancel/${orderId}`, 'POST'),
    updateOrderStatus: (data) => apiCall('/Order/status', 'PUT', data),
    getAllOrders: (status) => apiCall(`/Admin/orders?status=${status || ''}`, 'GET'),
    createInStoreOrder: (data) => apiCall('/Order/instore', 'POST', data),
    
    // ========== ADMIN ==========
    getDashboard: () => apiCall('/Admin/dashboard', 'GET'),
    
    // ========== WALLET ==========
    getWallet: async () => {
        console.log('📡 Fetching wallet...');
        try {
            const wallet = await apiCall('/Wallet', 'GET');
            console.log('✅ Wallet balance:', wallet.balance);
            return wallet;
        } catch (error) {
            console.error('❌ Error fetching wallet:', error);
            return { balance: 0 };
        }
    },
    getTransactions: async () => {
        console.log('📡 Fetching transactions...');
        try {
            const transactions = await apiCall('/Wallet/transactions', 'GET');
            console.log(`✅ Received ${transactions?.length || 0} transactions`);
            return transactions || [];
        } catch (error) {
            console.error('❌ Error fetching transactions:', error);
            return [];
        }
    },
    deposit: (data) => apiCall('/Wallet/deposit', 'POST', data),
    
    // ========== PAYMENT ==========
    payOrder: (orderId) => apiCall(`/Payment/pay/${orderId}`, 'POST'),
    
    // ========== SHIPPER ==========
    getAvailableOrders: () => {
        console.log('📡 Fetching available orders...');
        return apiCall('/Shipper/available-orders', 'GET');
    },
    takeOrder: (orderId) => {
        console.log(`📡 Taking order: ${orderId}`);
        return apiCall(`/Shipper/take/${orderId}`, 'POST');
    },
    updateShippingStatus: (data) => {
        console.log(`📡 Updating shipping status:`, data);
        return apiCall('/Shipper/status', 'PUT', data);
    },
    getMyShippingOrders: async (status = '') => {
        try {
            console.log(`📡 Fetching my shipping orders with status: ${status || 'all'}`);
            const response = await apiCall(`/Shipper/my-orders?status=${status}`, 'GET');
            console.log(`✅ Received ${response?.length || 0} orders`);
            return response;
        } catch (error) {
            console.log('⚠️ Cannot get my shipping orders:', error.message);
            return [];
        }
    },
}; 

// Export for use in other files
window.API = API;