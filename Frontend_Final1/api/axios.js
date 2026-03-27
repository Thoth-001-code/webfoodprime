// api/axios.js
const axiosInstance = {
    async request(endpoint, method = 'GET', data = null, isFormData = false) {
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
            config.body = isFormData ? data : JSON.stringify(data);
        }
        
        try {
            if (CONFIG.DEBUG) {
                console.log(`📡 ${method} ${CONFIG.API_BASE_URL}${endpoint}`);
            }
            
            const response = await fetch(`${CONFIG.API_BASE_URL}${endpoint}`, config);
            const text = await response.text();
            
            let result;
            try {
                result = JSON.parse(text);
            } catch {
                result = text;
            }
            
            if (!response.ok) {
                const errorMsg = typeof result === 'object' ? 
                    (result.message || result.title || JSON.stringify(result)) : 
                    result || `HTTP ${response.status}`;
                throw new Error(errorMsg);
            }
            
            return result;
        } catch (error) {
            console.error('❌ API Error:', error);
            throw error;
        }
    },
    
    get: (endpoint) => this.request(endpoint, 'GET'),
    post: (endpoint, data, isFormData = false) => this.request(endpoint, 'POST', data, isFormData),
    put: (endpoint, data, isFormData = false) => this.request(endpoint, 'PUT', data, isFormData),
    delete: (endpoint) => this.request(endpoint, 'DELETE'),
};

window.axiosInstance = axiosInstance;