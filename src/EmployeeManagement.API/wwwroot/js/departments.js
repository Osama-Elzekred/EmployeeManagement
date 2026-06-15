// departments.js - Department Management Logic

let currentDepartmentId = null;

async function loadDepartments() {
    const tbody = document.getElementById('departments-tbody');
    tbody.innerHTML = `
        <tr>
            <td colspan="4" class="text-center py-5">
                <p class="text-on-surface-variant font-body-md mt-2 mb-0">Loading departments...</p>
            </td>
        </tr>`;

    const response = await api.get('/departments');

    if (!response.success) {
        tbody.innerHTML = `<tr><td colspan="4" class="text-center py-5 text-error font-body-md">Failed to load departments: ${response.error}</td></tr>`;
        return;
    }

    renderDepartmentTable(response.data);
}

function renderDepartmentTable(departments) {
    const tbody = document.getElementById('departments-tbody');
    
    if (!departments || departments.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="text-center py-5">
                    <div class="flex flex-col items-center justify-center text-on-surface-variant">
                        <span class="material-symbols-outlined text-[48px] mb-2">domain_disabled</span>
                        <p class="font-body-md">No departments found.</p>
                    </div>
                </td>
            </tr>`;
        return;
    }

    let html = '';
    departments.forEach((dept) => {
        html += `
            <tr class="border-b border-[#f3f4f6] hover:bg-[#fcf8fa] transition-colors group">
                <td class="py-md px-lg font-body-md text-body-md text-on-surface-variant h-[64px]">#${dept.id}</td>
                <td class="py-md px-lg font-body-md text-body-md font-medium text-primary h-[64px]">
                    <div class="flex items-center gap-sm">
                        <span class="material-symbols-outlined text-outline text-[20px]">domain</span>
                        ${escapeHtml(dept.name)}
                    </div>
                </td>
                <td class="py-md px-lg h-[64px]">
                    <span class="inline-flex items-center px-2 py-1 rounded-full font-label-sm text-label-sm border border-gray-200 bg-gray-50 text-on-surface">
                        ${dept.employeeCount} Members
                    </span>
                </td>
                <td class="py-md px-lg h-[64px] text-right">
                    <div class="flex justify-end gap-sm row-hover-actions">
                        <button onclick="openEditDepartmentModal(${dept.id})" class="w-8 h-8 rounded-md flex items-center justify-center text-outline hover:text-primary hover:bg-gray-100 transition-colors" title="Edit">
                            <span class="material-symbols-outlined text-[18px]">edit</span>
                        </button>
                        <button onclick="confirmDeleteDepartment(${dept.id})" class="w-8 h-8 rounded-md flex items-center justify-center text-outline hover:text-error hover:bg-red-50 transition-colors" title="Delete" ${dept.employeeCount > 0 ? 'disabled' : ''}>
                            <span class="material-symbols-outlined text-[18px]">delete</span>
                        </button>
                    </div>
                </td>
            </tr>`;
    });

    tbody.innerHTML = html;
}

// Also used by employees.js to populate the dropdown
async function loadDepartmentsForDropdowns() {
    const response = await api.get('/departments');
    if (!response.success) return;

    const selectSearch = document.getElementById('search-department');
    const selectForm = document.getElementById('emp-department');
    
    // Keep first option
    selectSearch.innerHTML = '<option value="">All Departments</option>';
    selectForm.innerHTML = '<option value="" disabled selected>Select department</option>';

    response.data.forEach(dept => {
        selectSearch.innerHTML += `<option value="${dept.id}">${escapeHtml(dept.name)}</option>`;
        selectForm.innerHTML += `<option value="${dept.id}">${escapeHtml(dept.name)}</option>`;
    });
}

/**
 * Modals & Forms
 */
function openAddDepartmentModal() {
    currentDepartmentId = null;
    document.getElementById('department-form').reset();
    document.getElementById('dept-modal-error').classList.add('hidden');
    document.getElementById('departmentModalLabel').textContent = 'Add Department';
    document.getElementById('btn-save-dept-text').textContent = 'Save Department';
    
    openModal('departmentModal');
}

async function openEditDepartmentModal(id) {
    currentDepartmentId = id;
    document.getElementById('department-form').reset();
    document.getElementById('dept-modal-error').classList.add('hidden');
    document.getElementById('departmentModalLabel').textContent = 'Edit Department';
    document.getElementById('btn-save-dept-text').textContent = 'Update Department';

    openModal('departmentModal');

    const response = await api.get(`/departments/${id}`);

    if (response.success) {
        document.getElementById('dept-name').value = response.data.name;
    } else {
        closeModal('departmentModal');
        showToast(`Failed to load department: ${response.error}`, true);
    }
}

async function saveDepartment() {
    const form = document.getElementById('department-form');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const errorBanner = document.getElementById('dept-modal-error');
    errorBanner.classList.add('hidden');
    
    const btn = document.getElementById('btn-save-department');
    btn.disabled = true;

    const payload = {
        name: document.getElementById('dept-name').value
    };

    const isEdit = currentDepartmentId !== null;
    const response = isEdit 
        ? await api.put(`/departments/${currentDepartmentId}`, payload)
        : await api.post('/departments', payload);

    btn.disabled = false;

    if (response.success) {
        closeModal('departmentModal');
        showToast(`Department successfully ${isEdit ? 'updated' : 'added'}.`);
        loadDepartments();
        loadDepartmentsForDropdowns(); // refresh dropdowns
    } else {
        let errorMsg = response.message || response.error || 'An error occurred';
        if (response.errors && response.errors.length > 0) {
            errorMsg += ': ' + response.errors.join(', ');
        }
        errorBanner.textContent = errorMsg;
        errorBanner.classList.remove('hidden');
    }
}

/**
 * Delete
 */
function confirmDeleteDepartment(id) {
    currentDepartmentId = id;
    document.getElementById('delete-message').textContent = 'Are you sure you want to delete this department?';
    document.getElementById('btn-confirm-delete').onclick = executeDeleteDepartment;
    openModal('deleteModal');
}

async function executeDeleteDepartment() {
    const btn = document.getElementById('btn-confirm-delete');
    btn.disabled = true;

    try {
        const response = await api.delete(`/departments/${currentDepartmentId}`);
        closeModal('deleteModal');
        showToast('Department deleted successfully.');
        loadDepartments();
        loadDepartmentsForDropdowns();
    } catch (error) {
        closeModal('deleteModal');
        showToast(`Delete failed: ${error.message || 'An error occurred'}`, true);
    } finally {
        btn.disabled = false;
    }
}
