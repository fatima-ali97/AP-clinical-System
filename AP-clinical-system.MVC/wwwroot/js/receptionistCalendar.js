document.addEventListener('DOMContentLoaded', function () {
    const timeAxisEl = document.getElementById('timeAxis');
    const doctorColumnsEl = document.getElementById('doctorColumns');
    const dateFilterEl = document.getElementById('calendarDate');

    const slotStartVal = 1000;
    const totalSlots = 18;

    function formatTime(slotVal) {
        if (!window.TimeSlotPicker) return '';
        return window.TimeSlotPicker.slotToTime(slotVal);
    }

    function renderTimeAxis() {
        timeAxisEl.innerHTML = '<div class="doctor-header">Time</div>';
        for (let i = 0; i < totalSlots; i++) {
            const timeLabel = formatTime(slotStartVal + i);
            const slotEl = document.createElement('div');
            slotEl.className = 'time-slot-label';
            slotEl.textContent = timeLabel;
            timeAxisEl.appendChild(slotEl);
        }
    }

    function renderCalendar() {
        const selectedDate = dateFilterEl.value;
        const selectedDoctorIds = $('#calendarDoctorFilter').val() || [];

        doctorColumnsEl.innerHTML = ''; 

        const filteredDoctors = doctorsData.filter(d => selectedDoctorIds.includes(d.id));

        filteredDoctors.forEach(doctor => {
            const col = document.createElement('div');
            col.className = 'doctor-column';
            col.dataset.doctorId = doctor.id;

            const header = document.createElement('div');
            header.className = 'doctor-header';
            header.textContent = doctor.name;
            col.appendChild(header);

            const slotsContainer = document.createElement('div');
            slotsContainer.className = 'doctor-slots';

            for (let i = 0; i < totalSlots; i++) {
                const slotVal = slotStartVal + i;
                const slotEl = document.createElement('div');
                slotEl.className = 'calendar-slot';
                slotEl.dataset.slotVal = slotVal;

                const appt = appointmentsData.find(a =>
                    a.doctorId === doctor.id &&
                    a.date === selectedDate &&
                    a.slot === slotVal &&
                    a.status !== 5 
                );

                if (appt) {
                    slotEl.classList.add('booked');
                    const statusStr = statusMap[appt.status] || 'requested';

                    const card = document.createElement('div');
                    card.className = `appointment-card status-${statusStr}`;
                    if (statusStr === 'requested') {
                        card.classList.add('status-requested-trigger');
                        card.dataset.apptId = appt.id;
                        card.dataset.patientName = appt.patientName;
                        card.dataset.doctorUserId = doctor.userId;
                        card.style.cursor = 'pointer';
                    }

                    const cardColors = [
                        'var(--color-slate-100)', 'var(--color-slate-200)', 'var(--color-slate-300)', 'var(--color-slate-400)', 'var(--color-slate-500)', 'var(--color-slate-600)', 'var(--color-slate-700)', 'var(--color-slate-800)', 'var(--color-slate-900)', 'var(--color-slate-950)',
                        'var(--color-indigo-50)', 'var(--color-indigo-100)', 'var(--color-indigo-200)', 'var(--color-indigo-300)', 'var(--color-indigo-400)', 'var(--color-indigo-500)', 'var(--color-indigo-600)', 'var(--color-indigo-700)', 'var(--color-indigo-800)', 'var(--color-indigo-900)', 'var(--color-indigo-950)'
                    ];
                    const randIdx = Math.floor(Math.random() * cardColors.length);
                    const isLight = (randIdx <= 3) || (randIdx >= 10 && randIdx <= 14);

                    card.style.backgroundColor = cardColors[randIdx];
                    card.style.color = isLight ? 'var(--color-slate-900, #0f172a)' : '#fff';
                    card.style.border = isLight ? '1px solid var(--border-color, #e2e8f0)' : '1px solid transparent';
                    card.style.position = 'relative';

                    card.innerHTML = `
                        <div class="appt-title">${appt.patientName}</div>
                        <div class="appt-sub" style="display: flex; justify-content: space-between; align-items: center;">
                            <span>${formatTime(slotVal)}</span>
                            <div class="appt-card-actions" style="display: flex; gap: 8px;">
                                <i class="bi bi-arrow-repeat action-icon cal-status-btn"
                                   data-appt-id="${appt.id}"
                                   data-status="${statusStr}"
                                   title="Change Status"
                                   style="cursor: pointer; font-size: 1.1rem; opacity: 0.9;"></i>
                                <i class="bi bi-calendar-event action-icon reschedule-trigger-btn"
                                   data-appt-id="${appt.id}"
                                   data-doctor-user-id="${doctor.userId}"
                                   title="Reschedule"
                                   style="cursor: pointer; font-size: 1.1rem; opacity: 0.9;"></i>
                                <i class="bi bi-x-circle action-icon cal-cancel-btn"
                                   data-appt-id="${appt.id}"
                                   title="Cancel Appointment"
                                   style="cursor: pointer; font-size: 1.1rem; opacity: 0.9; color: #ef4444;"></i>
                            </div>
                        </div>
                    `;
                    slotEl.appendChild(card);
                } else {
                    const addBtn = document.createElement('button');
                    addBtn.className = 'add-appt-btn';
                    addBtn.innerHTML = '+';
                    addBtn.title = `Book ${doctor.name} at ${formatTime(slotVal)}`;

                    const capturedDoctor = doctor;
                    const capturedSlotVal = slotVal;
                    const capturedDate = selectedDate;

                    addBtn.addEventListener('click', function (e) {
                        e.stopPropagation();
                        const addModalBackdrop = document.getElementById('apptModalBackdrop');
                        if (!addModalBackdrop) return;

                        addModalBackdrop.classList.add('open');
                        document.body.style.overflow = 'hidden';

                        setTimeout(() => {
                            const specSel = document.getElementById('apptSpecialization');
                            if (specSel && capturedDoctor.specializationId) {
                                specSel.value = capturedDoctor.specializationId;
                                specSel.dispatchEvent(new Event('change'));
                            }

                            const doctorSel = document.getElementById('apptDoctor');
                            if (doctorSel) {
                                doctorSel.innerHTML = '';
                                const opt = document.createElement('option');
                                opt.value = capturedDoctor.id;
                                opt.dataset.userId = capturedDoctor.userId;
                                opt.textContent = capturedDoctor.name;
                                opt.selected = true;
                                doctorSel.appendChild(opt);
                                doctorSel.disabled = false;
                            }

                            const dateInput = document.getElementById('apptDate');
                            if (dateInput) {
                                dateInput.value = capturedDate;
                                dateInput.disabled = false;
                            }

                            if (window.TimeSlotPicker && capturedDoctor.userId && capturedDate) {
                                const slotGrid = document.getElementById('apptSlotGrid');
                                const slotInput = document.getElementById('apptSlotInput');
                                window.TimeSlotPicker.loadSlots(
                                    capturedDoctor.userId,
                                    capturedDate,
                                    slotGrid,
                                    slotInput
                                ).then(() => {
                                    const slotBtn = slotGrid?.querySelector(`[data-slot-value="${capturedSlotVal}"]`);
                                    if (slotBtn) slotBtn.click();
                                }).catch(() => {
                                    setTimeout(() => {
                                        const slotBtn = slotGrid?.querySelector(`[data-slot-value="${capturedSlotVal}"]`);
                                        if (slotBtn) slotBtn.click();
                                    }, 600);
                                });
                            }
                        }, 80);
                    });
                    slotEl.appendChild(addBtn);
                }

                slotsContainer.appendChild(slotEl);
            }

            col.appendChild(slotsContainer);
            doctorColumnsEl.appendChild(col);
        });
    }

    $('#calendarDoctorFilter').on('change', renderCalendar);
    dateFilterEl.addEventListener('change', renderCalendar);

    if (window.TimeSlotPicker) {
        renderTimeAxis();
        renderCalendar();
    } else {
        setTimeout(() => {
            renderTimeAxis();
            renderCalendar();
        }, 100);
    }
});