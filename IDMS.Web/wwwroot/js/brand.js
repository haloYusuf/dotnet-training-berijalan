let state = {
    page: 1,
    search: '',
    limit: 10
};

let searchTimeout = null;
let isLoading = false;

const $ = id => document.getElementById(id);
const html = (str, ctx) => str.replace(/\$\{(\w+)\}/g, (_, k) => ctx[k] ?? '');

function debounce(fn, ms) {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(fn, ms);
}

function showToast(message, type = 'error') {
    const toast = document.createElement('div');
    const colors = {
        error: 'bg-red-500',
        success: 'bg-emerald-500',
        warning: 'bg-amber-500'
    };
    toast.className = `fixed top-4 right-4 z-50 ${colors[type]} text-white px-5 py-3 rounded-lg shadow-lg text-sm font-medium transition-all duration-300 translate-y-0 opacity-0`;
    toast.textContent = message;
    document.body.appendChild(toast);
    requestAnimationFrame(() => toast.classList.remove('opacity-0'));
    setTimeout(() => {
        toast.classList.add('opacity-0');
        setTimeout(() => toast.remove(), 300);
    }, 4000);
}

function showSkeleton() {
    $('brandTableBody').innerHTML = Array.from({ length: 5 }, () => `
        <tr class="border-b border-gray-100">
            <td class="py-3 px-4"><div class="h-4 bg-gray-200 rounded w-12 animate-pulse"></div></td>
            <td class="py-3 px-4"><div class="h-4 bg-gray-200 rounded w-48 animate-pulse"></div></td>
            <td class="py-3 px-4"><div class="h-5 bg-gray-200 rounded-full w-16 animate-pulse"></div></td>
            <td class="py-3 px-4"><div class="h-4 bg-gray-200 rounded w-24 animate-pulse"></div></td>
        </tr>
    `).join('');
}

async function loadBrands() {
    if (isLoading) return;
    isLoading = true;

    state.search = $('searchInput').value;
    $('pagination').classList.add('hidden');
    showSkeleton();

    try {
        const params = new URLSearchParams({ search: state.search, page: state.page, limit: state.limit });
        const res = await fetch(`/Brand/List?${params}`);
        const json = await res.json();

        if (!res.ok || json.status === 'Error') {
            if (res.status === 401) return window.location.href = '/Auth/Login';
            showToast(json.message || `Server error (${res.status})`);
            $('brandTableBody').innerHTML = `<tr><td colspan="4" class="py-16 text-center text-gray-400 text-sm">Failed to load data</td></tr>`;
            return;
        }

        const items = json.data;
        if (!items?.length) {
            $('brandTableBody').innerHTML = `<tr><td colspan="4" class="py-16 text-center text-gray-400 text-sm">
                <div class="flex flex-col items-center gap-2">
                    <svg class="w-10 h-10 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"/></svg>
                    <span>${state.search ? `No brands matching "${state.search}"` : 'No brands yet'}</span>
                </div>
            </td></tr>`;
        } else {
            $('brandTableBody').innerHTML = items.map(item => `
                <tr class="border-b border-gray-50 hover:bg-gray-50/80 transition-colors">
                    <td class="py-3.5 px-4"><span class="font-mono text-sm font-medium text-gray-900">${esc(item.code)}</span></td>
                    <td class="py-3.5 px-4 text-gray-700">${esc(item.name)}</td>
                    <td class="py-3.5 px-4">${item.isActive
                    ? '<span class="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-emerald-50 text-emerald-700 ring-1 ring-inset ring-emerald-600/20"><span class="w-1.5 h-1.5 rounded-full bg-emerald-500"></span>Active</span>'
                    : '<span class="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-red-50 text-red-700 ring-1 ring-inset ring-red-600/20"><span class="w-1.5 h-1.5 rounded-full bg-red-400"></span>Inactive</span>'}
                    </td>
                    <td class="py-3.5 px-4">
                        <button onclick="editBrand(${item.id})" class="text-indigo-600 hover:text-indigo-800 font-medium text-sm mr-2 transition-colors">Edit</button>
                        <button onclick="deleteBrand(${item.id})" class="text-red-500 hover:text-red-700 font-medium text-sm transition-colors">Delete</button>
                    </td>
                </tr>
            `).join('');
        }

        renderPagination(json.pagination);
    } catch {
        showToast('Network error. Please check your connection.');
        $('brandTableBody').innerHTML = `<tr><td colspan="4" class="py-16 text-center text-gray-400 text-sm">Connection failed</td></tr>`;
    } finally {
        isLoading = false;
    }
}

function renderPagination(p) {
    const el = $('pagination');
    if (!p || p.totalPages <= 1) return el.classList.add('hidden');
    el.classList.remove('hidden');

    const pages = [];
    for (let i = 1; i <= p.totalPages; i++) pages.push(i);

    el.innerHTML = `
        <div class="text-sm text-gray-500">
            Page <span class="font-medium text-gray-700">${p.currentPage}</span> of <span class="font-medium text-gray-700">${p.totalPages}</span>
            &middot; ${p.totalItems} items
        </div>
        <div class="flex items-center gap-1">
            <button onclick="goToPage(${p.currentPage - 1})" ${p.hasPreviousPage ? '' : 'disabled'}
                class="px-2.5 py-1.5 rounded-md text-sm font-medium transition-colors ${p.hasPreviousPage ? 'text-gray-600 hover:bg-gray-100' : 'text-gray-300 cursor-not-allowed'}">
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/></svg>
            </button>
            ${pages.map(i => `
                <button onclick="goToPage(${i})"
                    class="w-8 h-8 rounded-md text-sm font-medium transition-colors ${i === p.currentPage ? 'bg-indigo-600 text-white shadow-sm' : 'text-gray-600 hover:bg-gray-100'}">${i}</button>
            `).join('')}
            <button onclick="goToPage(${p.currentPage + 1})" ${p.hasNextPage ? '' : 'disabled'}
                class="px-2.5 py-1.5 rounded-md text-sm font-medium transition-colors ${p.hasNextPage ? 'text-gray-600 hover:bg-gray-100' : 'text-gray-300 cursor-not-allowed'}">
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>
            </button>
        </div>`;
}

function goToPage(page) {
    state.page = page;
    loadBrands();
}

function onSearchInput() {
    state.page = 1;
    debounce(loadBrands, 300);
}

function clearSearch() {
    $('searchInput').value = '';
    state.page = 1;
    loadBrands();
}

function esc(str) {
    const d = document.createElement('div');
    d.textContent = str;
    return d.innerHTML;
}

function openModal(title, data = null) {
    hideModalError();

    const modal = $('modalOverlay');
    $('modalTitle').textContent = title;
    $('brandId').value = data?.id ?? '';
    $('brandCode').value = data?.code ?? '';
    $('brandName').value = data?.name ?? '';
    modal.classList.remove('hidden');
    modal.classList.add('flex');
    setTimeout(() => $('brandCode').focus(), 100);
}

function closeModal() {
    $('modalOverlay').classList.add('hidden');
    $('modalOverlay').classList.remove('flex');
    hideModalError();
}

function showModalError(msg) {
    const el = $('modalError');
    $('modalErrorText').textContent = msg;
    el.classList.remove('hidden');
}

function hideModalError() {
    $('modalError').classList.add('hidden');
}

function openCreateModal() {
    openModal('Create Brand');
}

async function editBrand(id) {
    try {
        const res = await fetch(`/Brand/Detail/${id}`);
        if (!res.ok) {
            if (res.status === 401) return window.location.href = '/Auth/Login';
            const err = await res.json().catch(() => ({}));
            return showToast(err.message || `Server error (${res.status})`);
        }
        const json = await res.json();
        if (json.status === 'Error') return showToast(json.message);
        openModal('Edit Brand', json.data);
    } catch {
        showToast('Network error');
    }
}

let isSubmitting = false;

async function submitForm() {
    if (isSubmitting) return;
    hideModalError();

    const id = $('brandId').value;
    const code = $('brandCode').value.trim();
    const name = $('brandName').value.trim();

    if (!code) return showModalError('Code is required'), $('brandCode').focus();
    if (code.length > 3) return showModalError('Code cannot exceed 3 characters');
    if (!name) return showModalError('Name is required'), $('brandName').focus();

    const isEdit = !!id;
    const url = isEdit ? `/Brand/Update/${id}` : '/Brand/Create';
    const method = isEdit ? 'PUT' : 'POST';
    const btn = $('saveBtn');
    const originalText = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Saving...';
    isSubmitting = true;

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ code, name })
        });

        const json = await res.json();

        if (!res.ok || json.status === 'Error') {
            btn.disabled = false;
            btn.textContent = originalText;
            isSubmitting = false;
            return showModalError(json.message || `Server error (${res.status})`);
        }

        closeModal();
        state.page = 1;
        showToast(isEdit ? 'Brand updated' : 'Brand created', 'success');
        loadBrands();
    } catch {
        btn.disabled = false;
        btn.textContent = originalText;
        isSubmitting = false;
        showModalError('Network error');
    } finally {
    btn.disabled = false;
    btn.textContent = 'Save';
    isSubmitting = false;
    }
}

let deleteTargetId = null;

function openDeleteModal(id, name) {
    deleteTargetId = id;
    $('deleteBrandInfo').textContent = `"${name}" will be permanently removed.`;
    $('deleteModalOverlay').classList.remove('hidden');
    $('deleteModalOverlay').classList.add('flex');
}

function closeDeleteModal() {
    deleteTargetId = null;
    $('deleteModalOverlay').classList.add('hidden');
    $('deleteModalOverlay').classList.remove('flex');
    $('deleteError').classList.add('hidden');
}

function showDeleteError(msg) {
    $('deleteErrorText').textContent = msg;
    $('deleteError').classList.remove('hidden');
}

async function confirmDelete() {
    if (!deleteTargetId) return;
    showDeleteError('');

    try {
        const res = await fetch(`/Brand/Delete/${deleteTargetId}`, { method: 'DELETE' });
        const json = await res.json();

        if (!res.ok || json.status === 'Error') {
            if (res.status === 401) return window.location.href = '/Auth/Login';
            return showDeleteError(json.message || `Server error (${res.status})`);
        }

        closeDeleteModal();
        showToast('Brand deleted', 'success');
        loadBrands();
    } catch {
        showDeleteError('Network error');
    }
}

async function deleteBrand(id) {
    try {
        const res = await fetch(`/Brand/Detail/${id}`);
        if (!res.ok) {
            if (res.status === 401) return window.location.href = '/Auth/Login';
            const err = await res.json().catch(() => ({}));
            return showToast(err.message || `Server error (${res.status})`);
        }
        const json = await res.json();
        if (json.status === 'Error') return showToast(json.message);
        openDeleteModal(id, json.data?.name || 'Unknown');
    } catch {
        showToast('Network error');
    }
}

document.addEventListener('DOMContentLoaded', loadBrands);
