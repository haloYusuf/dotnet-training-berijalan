/**
 * Javascript untuk Pengiriman Kendaraan (Vanilla JS & Tailwind)
 */

let keyword = '';
let page = 1;
let limit = 10;

let tbody, form, modalEl, searchInput, paginationContainer;
let appDropdown, dealerDropdown, insDropdown;

document.addEventListener("DOMContentLoaded", function () {
    tbody = document.getElementById('delivTableBody');
    form = document.getElementById('delivForm');
    modalEl = document.getElementById('delivModal');
    searchInput = document.getElementById('searchInput');
    paginationContainer = document.getElementById('paginationContainer');

    appDropdown = document.getElementById('delivApplicationId');
    dealerDropdown = document.getElementById('delivDealerId');
    insDropdown = document.getElementById('delivInsuranceId');

    loadDropdowns();
    loadData();

    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') search();
    });
});

async function webCall(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: { 'Content-Type': 'application/json' }
    };
    if (data) options.body = JSON.stringify(data);

    const response = await fetch(url, options);
    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

    const result = await response.json();
    if (result.status === "Error") throw new Error(result.message);
    return result;
}

function formatDateForInput(dateString) {
    if (!dateString) return '';
    return dateString.split('T')[0];
}

// Convert "2024-01-01T14:30:00" ke format datetime-local HTML "2024-01-01T14:30"
function formatDateTimeForInput(dateString) {
    if (!dateString) return '';
    return dateString.substring(0, 16);
}

function formatDateTimeForDisplay(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('id-ID', { day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' });
}

// Mengambil relasi data untuk Dropdown Form
async function loadDropdowns() {
    try {
        const [appRes, dealerRes, insRes] = await Promise.all([
            webCall('/Application/List?page=1&limit=1000'),
            webCall('/Dealer/List?page=1&limit=1000'),
            webCall('/Insurance/List?page=1&limit=1000')
        ]);

        appDropdown.innerHTML = '<option value="">-- Pilih Aplikasi --</option>';
        appRes.data.forEach(a => {
            const opt = document.createElement('option');
            opt.value = a.id;
            opt.textContent = `ID: ${a.id} - Rp ${a.otrPrice}`;
            appDropdown.appendChild(opt);
        });

        dealerDropdown.innerHTML = '<option value="">-- Pilih Dealer --</option>';
        dealerRes.data.forEach(d => {
            const opt = document.createElement('option');
            opt.value = d.id;
            opt.textContent = `${d.code} - ${d.name}`;
            dealerDropdown.appendChild(opt);
        });

        insDropdown.innerHTML = '<option value="">-- Pilih Asuransi --</option>';
        insRes.data.forEach(i => {
            const opt = document.createElement('option');
            opt.value = i.id;
            opt.textContent = `${i.code} - ${i.name}`;
            insDropdown.appendChild(opt);
        });
    } catch (error) { console.error("Gagal memuat dropdowns:", error); }
}

async function loadData() {
    tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-gray-500">Memuat data...</td></tr>';
    try {
        const response = await webCall(`/VehicleDelivery/List?keyword=${encodeURIComponent(keyword)}&page=${page}&limit=${limit}`);
        renderTable(response.data);
        renderPagination(response.pagination);
    } catch (error) {
        tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-red-500">Terjadi kesalahan saat memuat data.</td></tr>';
        paginationContainer.classList.add('hidden');
    }
}

function renderTable(data) {
    tbody.innerHTML = '';
    if (!data || data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="px-6 py-8 text-center text-sm text-gray-500">Tidak ada data ditemukan.</td></tr>';
        return;
    }

    const statuses = ['PLANNED', 'IN_TRANSIT', 'DELIVERED', 'CANCELLED'];

    data.forEach((item) => {
        let statusOptions = '';
        statuses.forEach(s => {
            const isSelected = item.status === s ? 'selected' : '';
            statusOptions += `<option value="${s}" ${isSelected}>${s}</option>`;
        });

        let colorClass = 'text-gray-900 border-gray-300 bg-gray-50';
        if (item.status === 'IN_TRANSIT') colorClass = 'text-blue-800 border-blue-300 bg-blue-50 font-medium';
        else if (item.status === 'DELIVERED') colorClass = 'text-green-800 border-green-300 bg-green-50 font-medium';
        else if (item.status === 'CANCELLED') colorClass = 'text-red-800 border-red-300 bg-red-50 font-medium';

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">APP-${item.applicationId}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${formatDateTimeForDisplay(item.deliveryDate)}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="font-medium text-gray-900">${item.driverName} ${item.driverPhone ? `(${item.driverPhone})` : ''}</div>
                <div class="text-xs uppercase">${item.platNumber || 'Belum ada plat'}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div>Dealer: ${item.dealerName || `ID: ${item.dealerId}`}</div>
                <div class="text-xs">Asuransi: ${item.insuranceName || `ID: ${item.insuranceId}`}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-center">
                <select onchange="updateInlineStatus(${item.id}, this)" data-original="${item.status}" class="block w-full rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-xs py-1 px-2 border ${colorClass}">
                    ${statusOptions}
                </select>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-center text-sm font-medium">
                <button onclick="showEditModal(${item.id})" class="text-indigo-600 hover:text-indigo-900 bg-indigo-50 hover:bg-indigo-100 px-3 py-1 rounded-md mr-2 transition-colors">Edit</button>
                <button onclick="deleteData(${item.id})" class="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded-md transition-colors">Hapus</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

async function updateInlineStatus(id, selectElement) {
    const newStatus = selectElement.value;
    const oldStatus = selectElement.getAttribute('data-original');

    if (confirm(`Ubah status pengiriman menjadi ${newStatus}?`)) {
        try {
            await webCall(`/VehicleDelivery/UpdateStatus?id=${id}`, 'PUT', { status: newStatus });
            alert('Status berhasil diperbarui.');
            loadData();
        } catch (error) {
            alert(`Gagal mengubah status: ${error.message}`);
            selectElement.value = oldStatus;
        }
    } else {
        selectElement.value = oldStatus;
    }
}

function renderPagination(pagination) {
    const totalPages = pagination?.totalPages || pagination?.TotalPages || 0;
    const currentPage = pagination?.currentPage || pagination?.CurrentPage || page;
    const totalItems = pagination?.totalItems || pagination?.TotalItems || 0;

    if (totalPages <= 1) {
        paginationContainer.innerHTML = '';
        paginationContainer.classList.add('hidden');
        return;
    }

    paginationContainer.classList.remove('hidden');

    const prevDisabled = currentPage === 1 ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="changePage(${currentPage - 1})" class="hover:bg-gray-50 cursor-pointer"`;
    const nextDisabled = currentPage === totalPages ? 'disabled class="opacity-50 cursor-not-allowed"' : `onclick="changePage(${currentPage + 1})" class="hover:bg-gray-50 cursor-pointer"`;

    let pageButtons = '';
    for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage) {
            pageButtons += `<button aria-current="page" class="relative z-10 inline-flex items-center bg-indigo-600 px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">${i}</button>`;
        } else {
            pageButtons += `<button onclick="changePage(${i})" class="relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0">${i}</button>`;
        }
    }

    const startItem = ((currentPage - 1) * limit) + 1;
    const endItem = Math.min(currentPage * limit, totalItems);

    paginationContainer.innerHTML = `
        <div class="flex items-center justify-between w-full">
            <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
                <div>
                    <p class="text-sm text-gray-700">Menampilkan <span class="font-medium">${startItem}</span> - <span class="font-medium">${endItem}</span> dari <span class="font-medium">${totalItems}</span> hasil</p>
                </div>
                <div>
                    <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                        <button ${prevDisabled} class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0"><span class="sr-only">Previous</span><svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z" clip-rule="evenodd" /></svg></button>
                        ${pageButtons}
                        <button ${nextDisabled} class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0"><span class="sr-only">Next</span><svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"><path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" /></svg></button>
                    </nav>
                </div>
            </div>
        </div>
    `;
}

function changePage(newPage) { page = newPage; loadData(); }
function search() { keyword = searchInput.value; page = 1; loadData(); }
function showModal() { modalEl.classList.remove('hidden'); }
function hideModal() { modalEl.classList.add('hidden'); }

function showAddModal() {
    form.reset();
    document.getElementById('delivId').value = '';
    document.getElementById('delivStatus').value = 'PLANNED';
    document.getElementById('delivModalLabel').innerText = 'Tambah Pengiriman Baru';
    showModal();
}

async function showEditModal(id) {
    try {
        const response = await webCall(`/VehicleDelivery/Detail?id=${id}`);
        const data = response.data;

        document.getElementById('delivId').value = data.id;
        document.getElementById('delivApplicationId').value = data.applicationId;
        document.getElementById('delivDealerId').value = data.dealerId;
        document.getElementById('delivInsuranceId').value = data.insuranceId;
        document.getElementById('delivDeliveryDate').value = formatDateForInput(data.deliveryDate);
        document.getElementById('delivDriverName').value = data.driverName || '';
        document.getElementById('delivDriverPhone').value = data.driverPhone || '';
        document.getElementById('delivPlatNumber').value = data.platNumber || '';
        document.getElementById('delivStatus').value = data.status;
        document.getElementById('delivNotes').value = data.notes || '';

        document.getElementById('delivModalLabel').innerText = 'Edit Pengiriman';
        showModal();
    } catch (error) { alert(`Gagal mengambil data: ${error.message}`); }
}

async function save() {
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const id = document.getElementById('delivId').value;
    const isEdit = id !== '';

    const payload = {
        applicationId: parseInt(document.getElementById('delivApplicationId').value),
        dealerId: parseInt(document.getElementById('delivDealerId').value),
        insuranceId: parseInt(document.getElementById('delivInsuranceId').value),
        deliveryDate: new Date(document.getElementById('delivDeliveryDate').value).toISOString(),
        driverName: document.getElementById('delivDriverName').value,
        driverPhone: document.getElementById('delivDriverPhone').value,
        platNumber: document.getElementById('delivPlatNumber').value.toUpperCase(),
        status: document.getElementById('delivStatus').value,
        notes: document.getElementById('delivNotes').value
    };

    const method = isEdit ? 'PUT' : 'POST';
    const url = isEdit ? `/VehicleDelivery/Update?id=${id}` : `/VehicleDelivery/Create`;

    try {
        await webCall(url, method, payload);
        hideModal();
        alert(`Data berhasil ${isEdit ? 'diperbarui' : 'disimpan'}.`);
        loadData();
    } catch (error) { alert(`Gagal menyimpan data: ${error.message}`); }
}

async function deleteData(id) {
    if (confirm('Apakah Anda yakin ingin menghapus jadwal pengiriman ini?')) {
        try {
            await webCall(`/VehicleDelivery/Delete?id=${id}`, 'DELETE');
            alert('Data berhasil dihapus.');
            const currentRows = tbody.querySelectorAll('tr').length;
            if (currentRows === 1 && page > 1) page--;
            loadData();
        } catch (error) { alert(`Gagal menghapus data: ${error.message}`); }
    }
}