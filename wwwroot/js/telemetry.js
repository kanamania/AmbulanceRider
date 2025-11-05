// Initialize charts
let eventTypeChart = null;
let deviceTypeChart = null;
let map = null;
let marker = null;

// Render pie chart
export function renderPieChart(canvasId, title, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    
    // Destroy existing chart if it exists
    if (canvasId === 'eventTypeChart' && eventTypeChart) {
        eventTypeChart.destroy();
    } else if (canvasId === 'deviceTypeChart' && deviceTypeChart) {
        deviceTypeChart.destroy();
    }
    
    // Generate colors
    const backgroundColors = [
        'rgba(54, 162, 235, 0.7)',
        'rgba(255, 99, 132, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(255, 159, 64, 0.7)',
        'rgba(153, 102, 255, 0.7)',
        'rgba(255, 205, 86, 0.7)',
        'rgba(201, 203, 207, 0.7)'
    ];
    
    const borderColors = [
        'rgba(54, 162, 235, 1)',
        'rgba(255, 99, 132, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(255, 159, 64, 1)',
        'rgba(153, 102, 255, 1)',
        'rgba(255, 205, 86, 1)',
        'rgba(201, 203, 207, 1)'
    ];
    
    const chart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: backgroundColors.slice(0, labels.length),
                borderColor: borderColors.slice(0, labels.length),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                },
                title: {
                    display: true,
                    text: title,
                    font: {
                        size: 16
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.raw || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = Math.round((value / total) * 100);
                            return `${label}: ${value} (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
    
    // Store chart reference
    if (canvasId === 'eventTypeChart') {
        eventTypeChart = chart;
    } else if (canvasId === 'deviceTypeChart') {
        deviceTypeChart = chart;
    }
}

// Render bar chart
export function renderBarChart(canvasId, title, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    
    // Destroy existing chart if it exists
    if (canvasId === 'eventTypeChart' && eventTypeChart) {
        eventTypeChart.destroy();
    } else if (canvasId === 'deviceTypeChart' && deviceTypeChart) {
        deviceTypeChart.destroy();
    }
    
    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Count',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.7)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                title: {
                    display: true,
                    text: title,
                    font: {
                        size: 16
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.parsed.y} events`;
                        }
                    }
                }
            }
        }
    });
    
    // Store chart reference
    if (canvasId === 'eventTypeChart') {
        eventTypeChart = chart;
    } else if (canvasId === 'deviceTypeChart') {
        deviceTypeChart = chart;
    }
}

// Show map with marker
export async function showMap(latitude, longitude) {
    // Load Leaflet CSS if not already loaded
    if (!document.querySelector('link[href*="leaflet.css"]')) {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
        link.integrity = 'sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/mZEZx6QYVXJ0Z2a5A=';
        link.crossOrigin = '';
        document.head.appendChild(link);
    }
    
    // Load Leaflet JS if not already loaded
    if (!window.L) {
        await new Promise((resolve) => {
            const script = document.createElement('script');
            script.src = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
            script.integrity = 'sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=';
            script.crossOrigin = '';
            script.onload = resolve;
            document.head.appendChild(script);
        });
    }
    
    // Initialize or update map
    if (!map) {
        map = L.map('map').setView([latitude, longitude], 13);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);
    } else {
        map.setView([latitude, longitude], 13);
    }
    
    // Add or update marker
    if (marker) {
        marker.setLatLng([latitude, longitude]);
    } else {
        marker = L.marker([latitude, longitude]).addTo(map);
    }
    
    // Add popup with coordinates
    marker.bindPopup(`<b>Location</b><br>Lat: ${latitude.toFixed(6)}<br>Lng: ${longitude.toFixed(6)}`).openPopup();
}

// Clean up when component is disposed
export function dispose() {
    if (eventTypeChart) {
        eventTypeChart.destroy();
        eventTypeChart = null;
    }
    if (deviceTypeChart) {
        deviceTypeChart.destroy();
        deviceTypeChart = null;
    }
    if (map) {
        map.remove();
        map = null;
        marker = null;
    }
}
