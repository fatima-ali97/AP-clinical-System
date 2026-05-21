// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Counter Animation
document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".static");

    const startCounter = (counter) => {
        const target = +counter.getAttribute("data-target");
        let count = 0;

        const updateCounter = () => {
            const increment = Math.ceil(target / 100);

            if (count < target) {
                count += increment;

                if (count > target) count = target;

                counter.innerText = count;
                setTimeout(updateCounter, 20);
            }
        };

        updateCounter();
    };

    const observer = new IntersectionObserver((entries, obs) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const counters = entry.target.querySelectorAll(".counter");

                counters.forEach(counter => {
                    if (!counter.dataset.started) {
                        counter.dataset.started = "true";
                        startCounter(counter);
                    }
                });

                obs.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    cards.forEach(card => observer.observe(card));
});