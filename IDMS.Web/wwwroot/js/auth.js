const $ = id => document.getElementById(id);

function showError(msg) {
    const el = $('formError');
    el.querySelector('p').textContent = msg;
    el.classList.remove('hidden');
}

function hideError() {
    $('formError').classList.add('hidden');
}

function setLoading(isLoading) {
    const btn = $('submitBtn');
    btn.disabled = isLoading;
    btn.innerHTML = isLoading
        ? '<span class="inline-flex items-center gap-2"><svg class="animate-spin h-4 w-4" viewBox="0 0 24 24" fill="none"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>Please wait...</span>'
        : (btn.dataset.originalText || 'Submit');
}

async function handleLogin(e) {
    e.preventDefault();
    hideError();

    const email = $('email').value.trim();
    const password = $('password').value;

    if (!email) return showError('Email is required');
    if (!password) return showError('Password is required');

    setLoading(true);

    try {
        const res = await fetch('/Auth/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const json = await res.json();

        if (!res.ok || json.status === 'Error') {
            showError(json.message || 'Login failed');
            setLoading(false);
            return;
        }

        window.location.href = '/Brand';
    } catch {
        showError('Network error. Please check your connection.');
        setLoading(false);
    }
}

async function handleRegister(e) {
    e.preventDefault();
    hideError();

    const fullName = $('fullName').value.trim();
    const email = $('email').value.trim();
    const password = $('password').value;

    if (!fullName) return showError('Full name is required');
    if (!email) return showError('Email is required');
    if (!password) return showError('Password is required');
    if (password.length < 6) return showError('Password must be at least 6 characters');

    setLoading(true);

    try {
        const res = await fetch('/Auth/Register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, fullName })
        });

        const json = await res.json();

        if (!res.ok || json.status === 'Error') {
            showError(json.message || 'Registration failed');
            setLoading(false);
            return;
        }

        window.location.href = '/Auth/Login?registered=1';
    } catch {
        showError('Network error. Please check your connection.');
        setLoading(false);
    }
}
