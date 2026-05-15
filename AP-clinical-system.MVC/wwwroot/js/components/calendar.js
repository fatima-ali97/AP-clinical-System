
    // TODO: change this to fetch data or recieve them from C# handler or whatever
    const events = [
        { date: '2026-04-05', name: 'General Visit', time: '10:00 AM' },
        { date: '2026-04-05', name: 'General Visit', time: '04:10 PM' },
        { date: '2026-04-05', name: 'General Visit', time: '06:30 PM' },
        { date: '2026-04-12', name: 'Follow-up Visit', time: '09:00 AM' },
        { date: '2026-04-18', name: 'Lab Results Review', time: '02:30 PM' },
        { date: '2026-04-24', name: 'Specialist Consult', time: '11:00 AM' },
    ];

    let current = new Date(2026, 3, 1);
    let selectedDate = '2026-04-16';

    function formatKey(d) {
        return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0');
    }

    function changeMonth(dir) {
        current = new Date(current.getFullYear(), current.getMonth() + dir, 1);
        render();
    }

    function selectDay(key) {
        selectedDate = key;
        render();
    }

    function render() {
        const year = current.getFullYear();
        const month = current.getMonth();
        const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        document.getElementById('month-title').textContent = months[month] + ' ' + year;

        const grid = document.getElementById('days-grid');
        grid.innerHTML = '';

        const today = formatKey(new Date());
        const firstDay = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const prevDays = new Date(year, month, 0).getDate();

        const eventMap = {};
        events.forEach(ev => {
            if (!eventMap[ev.date]) eventMap[ev.date] = [];
            eventMap[ev.date].push(ev);
        });

        for (let i = 0; i < firstDay; i++) {
            const d = prevDays - firstDay + 1 + i;
            const cell = document.createElement('div');
            cell.className = 'day-cell other';
            cell.innerHTML = `<span class="day-num">${d}</span>`;
            grid.appendChild(cell);
        }

        for (let d = 1; d <= daysInMonth; d++) {
            const key = year + '-' + String(month + 1).padStart(2, '0') + '-' + String(d).padStart(2, '0');
            const cell = document.createElement('div');
            let cls = 'day-cell';
            if (key === today) cls += ' today';
            else if (key === selectedDate) cls += ' selected';
            cell.className = cls;
            const dots = eventMap[key] ? `<div class="dot-row">${eventMap[key].slice(0, 3).map((_, i) => `<div class="dot ${['blue', 'green', 'purple'][i % 3]}"></div>`).join('')}</div>` : '';
            cell.innerHTML = `<span class="day-num">${d}</span>${dots}`;
            cell.onclick = () => selectDay(key);
            grid.appendChild(cell);
        }

        const totalCells = firstDay + daysInMonth;
        const remaining = totalCells % 7 === 0 ? 0 : 7 - (totalCells % 7);
        for (let i = 1; i <= remaining; i++) {
            const cell = document.createElement('div');
            cell.className = 'day-cell other';
            cell.innerHTML = `<span class="day-num">${i}</span>`;
            grid.appendChild(cell);
        }

        const dayEvents = events.filter(ev => ev.date === selectedDate);
        const list = document.getElementById('events-list');
        const label = document.getElementById('events-label');

        if (selectedDate) {
            const [y, m, dd] = selectedDate.split('-');
            const dateObj = new Date(+y, +m - 1, +dd);
            const opts = { weekday: 'short', day: '2-digit', month: 'short', year: 'numeric' };
            label.textContent = dayEvents.length ? `Appointments — ${dateObj.toLocaleDateString('en-GB', opts)}` : 'No appointments';
        }

        list.innerHTML = '';
        if (!dayEvents.length) {
            list.innerHTML = '<div style="color: var(--color-text-tertiary); font-size: 14px; text-align:center; padding: 16px;">No appointments on this day</div>';
            return;
        }

        dayEvents.sort((a, b) => a.time.localeCompare(b.time));
        dayEvents.forEach((ev, idx) => {
            const card = document.createElement('div');
            card.className = 'event-card' + (idx === 0 ? ' highlighted' : '');
            card.innerHTML = `
                                                                                                                              <div class="event-name">${ev.name}</div>
                                                                                                                              <div class="event-time">
                                                                                                                                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                                                                                                                                ${['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'][new Date(...selectedDate.split('-').map((v, i) => i === 1 ? +v - 1 : +v)).getDay()]}, ${selectedDate.split('-')[2]} ${['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][+selectedDate.split('-')[1] - 1]} ${selectedDate.split('-')[0]}, ${ev.time}
                                                                                                                              </div>
                                                                                                                            `;
            list.appendChild(card);
        });
    }

    render();



    submitBtn.addEventListener('click', async () => {
        const payload = {
            appointment_reason: document.getElementById('reason').value,
            specialization_ref: document.getElementById('specialization').value, // swap for actual GUID
            doctor_ref: document.getElementById('doctor').value,                  // swap for actual GUID
            date: document.getElementById('appointmentDate').value,
            appointment_time_slot: selectedSlot,
            appointment_status: 0  // e.g. 0 = Pending
        };

        const response = await fetch('/Appointments/Book', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            // show success state
        }
    });