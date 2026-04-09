// Sync MkDocs theme with parent MediPrax app theme
(function() {
    function applyTheme(dark) {
        var body = document.body;
        var scheme = dark ? "slate" : "default";
        body.setAttribute("data-md-color-scheme", scheme);
        body.setAttribute("data-md-color-primary", "blue");
        body.setAttribute("data-md-color-accent", "blue");
    }

    // Check parent window theme on load
    function syncFromParent() {
        try {
            if (window.parent && window.parent !== window) {
                var parentTheme = window.parent.document.documentElement.getAttribute("data-theme");
                applyTheme(parentTheme === "dark");
            }
        } catch(e) {
            // Cross-origin — fall back to system preference
        }
    }

    // Listen for theme change messages from parent
    window.addEventListener("message", function(event) {
        if (event.data && event.data.type === "theme-change") {
            applyTheme(event.data.dark);
        }
    });

    // Initial sync
    syncFromParent();

    // Also observe parent for changes via polling (fallback)
    setInterval(syncFromParent, 1000);
})();
