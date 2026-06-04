// ── MediBook Site JS ──

// Slot picker: single-select
document.addEventListener('DOMContentLoaded', function () {

    // Slot selection
    document.querySelectorAll('.slot-btn:not(:disabled)').forEach(btn => {
        btn.addEventListener('click', function () {
            document.querySelectorAll('.slot-btn').forEach(b => b.classList.remove('selected', 'btn-primary'));
            this.classList.add('selected', 'btn-primary');
            const input = document.getElementById('selectedSlot');
            if (input) input.value = this.dataset.slot;
        });
    });

    // Auto-dismiss alerts after 5s
    document.querySelectorAll('.alert-dismissible').forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });

    // Confirm dangerous actions
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', function (e) {
            if (!confirm(this.dataset.confirm)) e.preventDefault();
        });
    });

    // Highlight active nav link
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-link').forEach(link => {
        const href = link.getAttribute('href');
        if (href && path.startsWith(href.toLowerCase()) && href !== '/') {
            link.classList.add('active');
        }
    });
});
