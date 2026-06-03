window.TimeSlotPicker = {
    slotToTime: function (sv) {
        const mins = (sv - 1000) * 30;
        const h = 8 + Math.floor(mins / 60);
        const m = mins % 60;
        const p = h >= 12 ? 'PM' : 'AM';
        const dh = h > 12 ? h - 12 : (h === 0 ? 12 : h);
        return `${String(dh).padStart(2, '0')}:${String(m).padStart(2, '0')} ${p}`;
    },
    loadSlots: async function (doctorUserId, date, gridElement, inputElement) {
        gridElement.innerHTML = '<p class="appt-slot-hint">Loading slots...</p>';
        inputElement.value = '';
        try {
            const res = await fetch('/Appointments/GetAvailableDatesAndTimesByDoctorID', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `doctorID=${encodeURIComponent(doctorUserId)}`
            });
            if (!res.ok) { 
                gridElement.innerHTML = '<p class="appt-slot-hint" style="color:var(--color-danger,#dc2626)">No schedule found for this doctor.</p>'; 
                return; 
            }
            const data = await res.json();
            const match = data.find(d => d.date === date);
            gridElement.innerHTML = '';
            if (!match || !match.availableTimeSlots.length) {
                gridElement.innerHTML = '<p class="appt-slot-hint">No available slots for this date.</p>'; 
                return;
            }
            match.availableTimeSlots.forEach(sv => {
                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'appt-slot';
                btn.dataset.slotValue = sv;
                btn.textContent = this.slotToTime(sv);
                btn.addEventListener('click', () => {
                    gridElement.querySelectorAll('.appt-slot').forEach(s => s.classList.remove('active'));
                    btn.classList.add('active');
                    inputElement.value = sv;
                });
                gridElement.appendChild(btn);
            });
        } catch {
            gridElement.innerHTML = '<p class="appt-slot-hint" style="color:var(--color-danger,#dc2626)">Error loading slots.</p>';
        }
    },
    clearSlots: function (gridElement, inputElement, hintText = 'Select a doctor and date to see available slots.') {
        gridElement.innerHTML = `<p class="appt-slot-hint">${hintText}</p>`;
        inputElement.value = '';
    }
};
