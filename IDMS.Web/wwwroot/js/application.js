/**
 * Javascript untuk Master Aplikasi (Vanilla JS & Tailwind)
 */

let keyword = '';
let page = 1;
let limit = 10;

let tbody;
let form;
let modalEl;
let searchInput;
let paginationContainer;
let customerDropdown;
let modelDropdown;

document.addEventListener("DOMContentLoaded", function () {
    tbody = document.getElementById('appTableBody');
    form = document.getElementById('appForm');
    modalEl = document.getElementById('appModal');
    searchInput = document.getElementById('searchInput');
    paginationContainer = document.getElementById('paginationContainer');
    customerDropdown = document.getElementById('appCustomerId');
    modelDropdown = document.getElementById('appModelId');

    loadCustomers();
    loadModels();
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

// Format Rupiah
function formatRupiah(number) {
    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR', minimumFractionDigits: 0 }).format(number);
}

// Load Dropdowns
async function loadCustomers() {
    try {
        const response = await webCall('/Customer/List?page=1&limit=1000');
        customerDropdown.innerHTML = '<option value="">-- Pilih Kustomer --</option>';
        response.data.forEach(c => {
            const option = document.createElement('option');
            option.value = c.id;
            option.textContent = `${c.nik} - ${c.fullName || c.name || ''}`;
            customerDropdown.appendChild(option);
        });
    } catch (error) { console.error("Gagal memuat kustomer"); }
}

async function loadModels() {
    try {
        const response = await webCall('/Model/List?page=1&limit=1000');
        modelDropdown.innerHTML = '<option value="">-- Pilih Model --</option>';
        response.data.forEach(m => {
            const option = document.createElement('option');
            option.value = m.id;
            option.textContent = `${m.code} - ${m.name}`;
            modelDropdown.appendChild(option);
        });
    } catch (error) { console.error("Gagal memuat model"); }
}

// Load Main Data
async function loadData() {
    tbody.innerHTML = '<tr><td colspan="7" class="px-6 py-8 text-center text-sm text-gray-500">Memuat data...</td></tr>';
    try {
        const response = await webCall(`/Application/List?keyword=${encodeURIComponent(keyword)}&page=${page}&limit=${limit}`);
        renderTable(response.data);
        renderPagination(response.pagination);
    } catch (error) {
        tbody.innerHTML = '<tr><td colspan="7" class="px-6 py-8 text-center text-sm text-red-500">Terjadi kesalahan saat memuat data.</td></tr>';
        paginationContainer.classList.add('hidden');
    }
}

function renderTable(data) {
    tbody.innerHTML = '';
    if (!data || data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="px-6 py-8 text-center text-sm text-gray-500">Tidak ada data ditemukan.</td></tr>';
        return;
    }

    const statuses = ['DRAFT', 'SUBMITTED', 'APPROVED', 'REJECTED'];

    data.forEach((item) => {
        // Membuat opsi select secara dinamis untuk status
        let statusOptions = '';
        statuses.forEach(s => {
            const isSelected = item.status === s ? 'selected' : '';
            statusOptions += `<option value="${s}" ${isSelected}>${s}</option>`;
        });

        // Warna styling untuk select
        let colorClass = 'text-gray-900 border-gray-300 bg-gray-50';
        if (item.status === 'APPROVED') colorClass = 'text-green-800 border-green-300 bg-green-50 font-medium';
        else if (item.status === 'REJECTED') colorClass = 'text-red-800 border-red-300 bg-red-50 font-medium';
        else if (item.status === 'SUBMITTED') colorClass = 'text-blue-800 border-blue-300 bg-blue-50 font-medium';

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">${item.applicationNo}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">${item.customerName || `ID: ${item.customerId}`}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${item.modelName || `ID: ${item.modelId}`}</td>
            <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-900">${formatRupiah(item.otrPrice)}</td>
            <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-900">${formatRupiah(item.dpAmount)}</td>
            <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500">
                <div>${item.tenorMonth} Bulan</div>
                <div class="text-xs text-gray-400">Bunga: ${item.interestRate}%</div>
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

// --- FUNGSI KHUSUS: Mengubah Status Melalui Dropdown Tabel ---
async function updateInlineStatus(id, selectElement) {
    const newStatus = selectElement.value;
    const oldStatus = selectElement.getAttribute('data-original');

    if (confirm(`Apakah Anda yakin ingin mengubah status aplikasi ini menjadi ${newStatus}?`)) {
        try {
            // Memanggil endpoint UpdateStatus (PUT /Application/UpdateStatus?id={id})
            await webCall(`/Application/UpdateStatus?id=${id}`, 'PUT', { status: newStatus });
            alert('Status berhasil diperbarui.');
            loadData(); // Memuat ulang agar warna dan style ikut berubah
        } catch (error) {
            alert(`Gagal mengubah status: ${error.message}`);
            selectElement.value = oldStatus; // Kembalikan ke nilai awal
        }
    } else {
        // Jika user membatalkan (Cancel), kembalikan ke nilai awal
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
                    <p class="text-sm text-gray-700">
                        Menampilkan <span class="font-medium">${startItem}</span> - <span class="font-medium">${endItem}</span> dari <span class="font-medium">${totalItems}</span> hasil
                    </p>
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
    document.getElementById('appId').value = '';
    document.getElementById('appStatus').value = 'DRAFT';
    document.getElementById('appModalLabel').innerText = 'Tambah Aplikasi Baru';
    showModal();
}

async function showEditModal(id) {
    try {
        const response = await webCall(`/Application/Detail?id=${id}`);
        const data = response.data;

        document.getElementById('appId').value = data.id;
        document.getElementById('appCustomerId').value = data.customerId;
        document.getElementById('appModelId').value = data.modelId;
        document.getElementById('appOtrPrice').value = data.otrPrice;
        document.getElementById('appDpAmount').value = data.dpAmount;
        document.getElementById('appTenorMonth').value = data.tenorMonth;
        document.getElementById('appInterestRate').value = data.interestRate;
        document.getElementById('appStatus').value = data.status;

        document.getElementById('appModalLabel').innerText = 'Edit Aplikasi';
        showModal();
    } catch (error) { alert(`Gagal mengambil data: ${error.message}`); }
}

async function save() {
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const id = document.getElementById('appId').value;
    console.log(id);
    const isEdit = id !== '';

    const payload = {
        customerId: parseInt(document.getElementById('appCustomerId').value),
        modelId: parseInt(document.getElementById('appModelId').value),
        otrPrice: parseFloat(document.getElementById('appOtrPrice').value),
        dpAmount: parseFloat(document.getElementById('appDpAmount').value),
        tenorMonth: parseInt(document.getElementById('appTenorMonth').value),
        interestRate: parseFloat(document.getElementById('appInterestRate').value),
        status: document.getElementById('appStatus').value
    };

    console.log(payload);

    const method = isEdit ? 'PUT' : 'POST';
    const url = isEdit ? `/Application/Update?id=${id}` : `/Application/Create`;

    try {
        await webCall(url, method, payload);
        hideModal();
        alert(`Data berhasil ${isEdit ? 'diperbarui' : 'disimpan'}.`);
        loadData();
    } catch (error) { alert(`Gagal menyimpan data: ${error.message}`); }
}

async function deleteData(id) {
    if (confirm('Apakah Anda yakin ingin menghapus data aplikasi ini?')) {
        try {
            await webCall(`/Application/Delete?id=${id}`, 'DELETE');
            alert('Data berhasil dihapus.');
            const currentRows = tbody.querySelectorAll('tr').length;
            if (currentRows === 1 && page > 1) page--;
            loadData();
        } catch (error) { alert(`Gagal menghapus data: ${error.message}`); }
    }
}