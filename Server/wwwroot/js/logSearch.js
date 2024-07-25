let originalLogContent = '';

function initialize() {
    const logContentElement = document.getElementById('logContent');
    if (logContentElement) {
        originalLogContent = logContentElement.innerText;
    }
}

function filterLogs() {
    const searchInput = document.getElementById('searchInput');
    const logContainer = document.getElementById('logContainer');

    if (searchInput && logContainer) {
        const searchQuery = searchInput.value.toLowerCase();
        const logLines = originalLogContent.split('\n');
        const filteredLines = logLines.filter(line => line.toLowerCase().includes(searchQuery));
        logContainer.innerText = filteredLines.join('\n');
    }
}

document.addEventListener("DOMContentLoaded", function () {
    initialize();
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('keyup', filterLogs);
    }
});
