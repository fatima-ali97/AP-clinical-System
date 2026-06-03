
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

    if (!editBtn || !backdrop) return;

    editBtn.addEventListener('click',   () => backdrop.classList.add('open'));
    closeBtn.addEventListener('click',  closeModal);
    cancelBtn.addEventListener('click', closeModal);
    backdrop.addEventListener('click',  e => { if (e.target === backdrop) closeModal(); });

    function closeModal() {
        backdrop.classList.remove('open');
    }

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
                displayName.textContent  = `${firstName} ${lastName}`.trim();
                displayPhone.textContent = phone || '—';

                const avatarEl = document.querySelector('.mp-avatar');
                if (avatarEl) {
                    avatarEl.textContent = ((firstName[0] ?? '') + (lastName[0] ?? '')).toUpperCase();
                }

                closeModal();
                Swal.fire({
    icon: 'success',
    title: 'Profile Updated',
    text: 'Your profile has been saved successfully.',
    confirmButtonColor: '#38789e',
    timer: 2000,
    timerProgressBar: true,
    showConfirmButton: false
});

            } else {
                Swal.fire({
    icon: 'error',
    title: 'Save Failed',
    text: 'Could not update your profile. Please try again.',
    confirmButtonColor: '#38789e'
});
            }
        } catch {
            showToast('Unexpected error. Please try again.', 'error');
        } finally {
            saveBtn.disabled    = false;
            saveBtn.textContent = 'Save Changes';
        }
    });

    function showToast(msg, type) {
        toast.textContent   = msg;
        toast.className     = `mp-toast ${type}`;
        toast.style.display = 'block';
        clearTimeout(toast._timer);
        toast._timer = setTimeout(() => { toast.style.display = 'none'; }, 3500);
    }

});
