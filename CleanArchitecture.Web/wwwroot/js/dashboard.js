document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("sidebar");
    const toggle = document.getElementById("sidebarToggle");
    const overlay = document.getElementById("sidebarOverlay");

    if (!sidebar || !toggle) return;

    function isMobile() {
        return window.innerWidth <= 991;
    }

    toggle.addEventListener("click", function () {

        if (isMobile()) {

            sidebar.classList.toggle("show");

            if (overlay) {
                overlay.classList.toggle("show");
            }

        } else {

            document.body.classList.toggle("sidebar-collapsed");

        }

    });

    if (overlay) {
        overlay.addEventListener("click", function () {
            sidebar.classList.remove("show");
            overlay.classList.remove("show");
        });
    }

});
