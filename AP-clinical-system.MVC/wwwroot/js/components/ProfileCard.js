
document.addEventListener('DOMContentLoaded', function () {

    const backdrop  = document.getElementById('mpModalBackdrop');
    const editBtn   = document.getElementById('mpEditBtn');
    const closeBtn  = document.getElementById('mpModalClose');
    const cancelBtn = document.getElementById('mpCancelBtn');
    const saveBtn   = document.getElementById('mpSaveBtn');
    const toast     = document.getElementById('mpToast');

    const inputFirst = document.getElementById('mpFirstName');
    const inputLast  = document.getElementById('mpLastName');
    const inputPhone = document.getElementById('mpPhone');

    const displayName  = document.getElementById('mp-display-name');
    const displayPhone = document.getElementById('mp-display-phone');

    // Guard: if profile card is not on this page, do nothing
    if (!editBtn || !backdrop) return;

    // ── open / close ──────────────────────────────────────────────────────
    editBtn.addEventListener('click',   () => backdrop.classList.add('open'));
    closeBtn.addEventListener('click',  closeModal);
    cancelBtn.addEventListener('click', closeModal);
    backdrop.addEventListener('click',  e => { if (e.target === backdrop) closeModal(); });

    function closeModal() {
        backdrop.classList.remove('open');
    }

    // ── save ──────────────────────────────────────────────────────────────
    saveBtn.addEventListener('click', async () => {
        const firstName = inputFirst.value.trim();
        const lastName  = inputLast.value.trim();
        const phone     = inputPhone.value.trim();

        if (!firstName || !lastName) {
            showToast('First and last name are required.', 'error');
            return;
        }

        saveBtn.disabled    = true;
        saveBtn.textContent = 'Saving…';

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';

            const res = await fetch('/Patient/UpdateProfile', {
                method:  'POST',
                headers: {
                    'Content-Type':             'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ firstName, lastName, phone })
            });

            const data = await res.json();

            if (data.success) {
                // update display values inline — no page reload needed
                displayName.textContent  = `${firstName} ${lastName}`.trim();
                displayPhone.textContent = phone || '—';

                // update avatar initials
                const avatarEl = document.querySelector('.mp-avatar');
                if (avatarEl) {
                    avatarEl.textContent = ((firstName[0] ?? '') + (lastName[0] ?? '')).toUpperCase();
                }

                closeModal();
                showToast('Profile updated successfully.', 'success');
            } else {
                showToast(data.error ?? 'Update failed.', 'error');
            }
        } catch {
            showToast('Unexpected error. Please try again.', 'error');
        } finally {
            saveBtn.disabled    = false;
            saveBtn.textContent = 'Save Changes';
        }
    });

    // ── toast helper ──────────────────────────────────────────────────────
    function showToast(msg, type) {
        toast.textContent   = msg;
        toast.className     = `mp-toast ${type}`;
        toast.style.display = 'block';
        clearTimeout(toast._timer);
        toast._timer = setTimeout(() => { toast.style.display = 'none'; }, 3500);
    }

});
