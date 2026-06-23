// Admin Dashboard JavaScript
$(document).ready(function() {
    $("#menu-toggle").click(function(e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-hide toast notifications after 5 seconds
    setTimeout(function() {
        $(".toast").fadeOut();
    }, 5000);

    // Load theme from localStorage
    if (localStorage.getItem('darkMode') === 'true') {
        $('body').addClass('dark-mode');
    }
});

// Toggle dark mode
function toggleTheme() {
    $('body').toggleClass('dark-mode');
    localStorage.setItem('darkMode', $('body').hasClass('dark-mode'));
}

// Confirm delete action
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

// Initialize charts (to be called on dashboard page)
function initializeCharts(userData, orderData) {
    // User Registration Chart
    const userCtx = document.getElementById('userChart').getContext('2d');
    new Chart(userCtx, {
        type: 'line',
        data: {
            labels: userData.map(d => d.label),
            datasets: [{
                label: 'User Registrations',
                data: userData.map(d => d.value),
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        }
    });

    // Order Status Chart
    const orderCtx = document.getElementById('orderChart').getContext('2d');
    new Chart(orderCtx, {
        type: 'doughnut',
        data: {
            labels: orderData.map(d => d.label),
            datasets: [{
                data: orderData.map(d => d.value),
                backgroundColor: [
                    'rgb(255, 99, 132)',
                    'rgb(54, 162, 235)',
                    'rgb(255, 205, 86)',
                    'rgb(75, 192, 192)',
                    'rgb(153, 102, 255)'
                ]
            }]
        }
    });
}