// Employee Management Functions

// Load departments for dropdown
async function loadDepartmentsForDropdowns() {
    try {
        const response = await apiGet('/departments');
        const departments = response.data;
        
        const select = document.getElementById('emp-department');
        if (select) {
            select.innerHTML = '<option value="" disabled selected>Select department</option>';
            departments.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                select.appendChild(option);
            });
        }

        const filterSelect = document.getElementById('search-department');
        if (filterSelect) {
            filterSelect.innerHTML = '<option value="">All Departments</option>';
            departments.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                filterSelect.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error loading departments:', error);
    }
}

// Load and display employees
async function loadEmployees() {
    try {
        const response = await apiGet('/employees');
        const employees = response.data;
        
        await updateEmployeeStats(employees);
        displayEmployees(employees);
    } catch (error) {
        console.error('Error loading employees:', error);
        showToast('Failed to load employees', true);
    }
}

// Display employees in table
function displayEmployees(employees) {
    const tbody = document.getElementById('employees-tbody');
    if (!tbody) return;
    
    if (!employees || employees.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center py-5">
                    <div class="flex flex-col items-center justify-center text-on-surface-variant">
                        <span class="material-symbols-outlined text-[48px] mb-2">person_off</span>
                        <p class="font-body-md">No employees found. Click "Add Employee" to create one.</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    // Color palette for avatars
    const colors = ['#141b2b', '#555f70', '#6366f1', '#8b5cf6', '#d946ef'];

    tbody.innerHTML = employees.map((emp, idx) => {
        // Generate initials
        const nameParts = emp.fullName.split(' ');
        const initials = (nameParts[0]?.[0] || '') + (nameParts[1]?.[0] || '');
        const avatarColor = colors[idx % colors.length];
        
        // Status color
        const statusColor = emp.isActive ? '#10b981' : '#9ca3af';
        const statusTextColor = emp.isActive ? '#047857' : '#6b7280';
        
        return `
        <tr class="border-b border-[#f3f4f6] hover:bg-[#fcf8fa] transition-colors group">
            <td class="py-md px-lg h-[64px]">
                <div class="flex items-center gap-md">
                    <div class="w-10 h-10 rounded-full text-white flex items-center justify-center font-label-md text-label-md font-bold shrink-0" style="background-color: ${avatarColor};">${escapeHtml(initials.toUpperCase())}</div>
                    <div class="flex flex-col">
                        <span class="font-body-md text-body-md font-medium text-primary">${escapeHtml(emp.fullName)}</span>
                        <span class="font-label-sm text-label-sm text-on-surface-variant font-normal">${escapeHtml(emp.email)}</span>
                    </div>
                </div>
            </td>
            <td class="py-md px-lg font-body-md text-body-md text-on-surface-variant h-[64px]">${escapeHtml(emp.mobileNumber)}</td>
            <td class="py-md px-lg h-[64px]">
                <span class="inline-flex items-center px-2 py-1 rounded-full font-label-sm text-label-sm border border-gray-200 bg-gray-50 text-on-surface">${escapeHtml(emp.departmentName)}</span>
            </td>
            <td class="py-md px-lg font-body-md text-body-md text-primary h-[64px]">${escapeHtml(emp.jobTitle)}</td>
            <td class="py-md px-lg font-body-md text-body-md text-on-surface-variant h-[64px]">${new Date(emp.hireDate).toLocaleDateString()}</td>
            <td class="py-md px-lg h-[64px]">
                <div class="flex items-center gap-xs">
                    <div class="w-2 h-2 rounded-full" style="background-color: ${statusColor};"></div>
                    <span class="font-label-md text-label-md" style="color: ${statusTextColor};">${emp.isActive ? 'Active' : 'Inactive'}</span>
                </div>
            </td>
            <td class="py-md px-lg h-[64px] text-right">
                <div class="flex justify-end gap-sm row-hover-actions">
                    <button onclick="editEmployee(${emp.id})" class="w-8 h-8 rounded-md flex items-center justify-center text-outline hover:text-primary hover:bg-gray-100 transition-colors" title="Edit">
                        <span class="material-symbols-outlined text-[18px]">edit</span>
                    </button>
                    <button onclick="deleteEmployee(${emp.id}, '${escapeHtml(emp.fullName)}')" class="w-8 h-8 rounded-md flex items-center justify-center text-outline hover:text-error hover:bg-red-50 transition-colors" title="Delete">
                        <span class="material-symbols-outlined text-[18px]">delete</span>
                    </button>
                </div>
            </td>
        </tr>
    `}).join('');
}

// Update employee statistics
async function updateEmployeeStats(employees) {
    const total = employees.length;
    const active = employees.filter(e => e.isActive).length;
    const inactive = total - active;
    
    const statTotal = document.getElementById('stat-total');
    const statActive = document.getElementById('stat-active');
    const statInactive = document.getElementById('stat-inactive');
    
    if (statTotal) statTotal.textContent = total;
    if (statActive) statActive.textContent = active;
    if (statInactive) statInactive.textContent = inactive;

    // Update department count
    try {
        const response = await apiGet('/departments');
        const departments = response.data;
        const statDepartments = document.getElementById('stat-departments');
        if (statDepartments) {
            statDepartments.textContent = departments.length;
        }
    } catch (error) {
        console.error('Error loading department count:', error);
    }
}

// Clear search and load all employees
function clearSearch() {
    document.getElementById('search-name').value = '';
    document.getElementById('search-department').value = '';
    loadEmployees();
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}

// Open add employee modal
function openAddModal() {
    const form = document.getElementById('employee-form');
    if (form) {
        document.getElementById('employee-id').value = '';
        form.reset();
        document.getElementById('employeeModalLabel').textContent = 'Add Employee';
        
        const today = new Date().toISOString().split('T')[0];
        const hireDate = document.getElementById('emp-hiredate');
        if (hireDate) hireDate.value = today;
    }
    
    openModal('employeeModal');
}

// Edit employee
async function editEmployee(id) {
    try {
        const response = await apiGet(`/employees/${id}`);
        const emp = response.data;
        
        document.getElementById('employee-id').value = emp.id;
        document.getElementById('emp-fullname').value = emp.fullName;
        document.getElementById('emp-email').value = emp.email;
        document.getElementById('emp-mobile').value = emp.mobileNumber;
        document.getElementById('emp-department').value = emp.departmentId;
        document.getElementById('emp-jobtitle').value = emp.jobTitle;
        document.getElementById('emp-hiredate').value = emp.hireDate.split('T')[0];
        document.getElementById('emp-isactive').checked = emp.isActive;
        
        document.getElementById('employeeModalLabel').textContent = 'Edit Employee';
        openModal('employeeModal');
    } catch (error) {
        showToast('Failed to load employee details', true);
    }
}

// Save employee (create or update)
async function saveEmployee() {
    const form = document.getElementById('employee-form');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const id = document.getElementById('employee-id').value;
    const data = {
        fullName: document.getElementById('emp-fullname').value,
        email: document.getElementById('emp-email').value,
        mobileNumber: document.getElementById('emp-mobile').value,
        departmentId: parseInt(document.getElementById('emp-department').value),
        jobTitle: document.getElementById('emp-jobtitle').value,
        hireDate: document.getElementById('emp-hiredate').value,
        isActive: document.getElementById('emp-isactive').checked
    };

    try {
        const btn = document.getElementById('btn-save-employee');
        btn.disabled = true;
        
        if (id) {
            await apiPut(`/employees/${id}`, data);
            showToast('Employee updated successfully!');
        } else {
            await apiPost('/employees', data);
            showToast('Employee created successfully!');
        }
        
        closeModal('employeeModal');
        await loadEmployees();
        btn.disabled = false;
    } catch (error) {
        showToast(error.message || 'Failed to save employee', true);
        const btn = document.getElementById('btn-save-employee');
        btn.disabled = false;
    }
}

// Delete employee
function deleteEmployee(id, name) {
    window.deleteId = id;
    window.deleteType = 'employee';
    document.getElementById('delete-message').textContent = `Are you sure you want to delete "${name}"?`;
    
    const btn = document.getElementById('btn-confirm-delete');
    btn.textContent = 'Delete';
    btn.onclick = confirmDelete;
    
    openModal('deleteModal');
}

// Confirm delete action
async function confirmDelete() {
    const id = window.deleteId;
    const type = window.deleteType;
    
    if (!id) return;
    
    try {
        const btn = document.getElementById('btn-confirm-delete');
        btn.disabled = true;
        
        if (type === 'employee') {
            await apiDelete(`/employees/${id}`);
            showToast('Employee deleted successfully!');
        } else if (type === 'department') {
            await apiDelete(`/departments/${id}`);
            showToast('Department deleted successfully!');
        }
        
        closeModal('deleteModal');
        
        if (type === 'employee') {
            await loadEmployees();
        } else {
            await loadDepartments();
            await loadDepartmentsForDropdowns();
        }
        
        btn.disabled = false;
    } catch (error) {
        showToast(error.message || 'Delete failed', true);
        const btn = document.getElementById('btn-confirm-delete');
        btn.disabled = false;
    }
}

// Search employees
async function searchEmployees() {
    const name = document.getElementById('search-name').value;
    const departmentId = document.getElementById('search-department').value;
    
    try {
        const endpoint = `/employees?${name ? `name=${encodeURIComponent(name)}` : ''}${departmentId ? `&departmentId=${departmentId}` : ''}`;
        const response = await apiGet(endpoint);
        displayEmployees(response.data);
    } catch (error) {
        showToast('Failed to search employees', true);
    }
}

// Clear employee search
function clearEmployeeSearch() {
    document.getElementById('searchName').value = '';
    document.getElementById('filterDepartment').value = '';
    loadEmployees();
}
