// API Configuration
const API_BASE_URL = '/api';

// Alert helper
function showAlert(message, type = 'success') {
    const alertId = 'alert-' + Date.now();
    const alertHtml = `
        <div class="alert alert-custom alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show" role="alert" id="${alertId}">
            <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    const container = document.getElementById('alert-container') || document.getElementById('alert-container-dept');
    if (container) {
        container.insertAdjacentHTML('beforeend', alertHtml);
        setTimeout(() => {
            const alert = document.getElementById(alertId);
            if (alert) alert.remove();
        }, 5000);
    }
}

// Fetch helper with error handling
async function apiFetch(endpoint, options = {}) {
    try {
        const url = `${API_BASE_URL}${endpoint}`;
        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });

        if (response.status === 204) {
            return { success: true, data: null };
        }

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || `API Error: ${response.status}`);
        }

        return data;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Generic GET request
async function apiGet(endpoint) {
    return apiFetch(endpoint, { method: 'GET' });
}

// Generic POST request
async function apiPost(endpoint, body) {
    return apiFetch(endpoint, {
        method: 'POST',
        body: JSON.stringify(body)
    });
}

// Generic PUT request
async function apiPut(endpoint, body) {
    return apiFetch(endpoint, {
        method: 'PUT',
        body: JSON.stringify(body)
    });
}

// Generic DELETE request
async function apiDelete(endpoint) {
    return apiFetch(endpoint, { method: 'DELETE' });
}

// API object for cleaner syntax (used by departments.js)
const api = {
    get: apiGet,
    post: apiPost,
    put: apiPut,
    delete: apiDelete
};
