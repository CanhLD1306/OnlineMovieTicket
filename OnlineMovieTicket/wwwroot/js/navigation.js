document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.toLowerCase();

    // Xử lý các menu con
    document.querySelectorAll('.collapse-item').forEach(function (item) {
        const href = item.getAttribute('href').toLowerCase();

        if (currentPath === href || currentPath.startsWith(href)) {
            // Thêm class active vào menu con
            item.classList.add('active');

            // Mở menu cha (thêm class show vào div collapse)
            const collapseDiv = item.closest('.collapse');
            if (collapseDiv) {
                collapseDiv.classList.add('show');
            }

            // Thêm class active vào nav-item cha
            const navItem = item.closest('.nav-item');
            if (navItem) {
                navItem.classList.add('active');
            }
        }
    });

    // Xử lý các menu không có collapse (trực tiếp nav-link)
    document.querySelectorAll('.nav-link').forEach(function (link) {
        const href = link.getAttribute('href');
        if (href && (currentPath === href.toLowerCase())) {
            const navItem = link.closest('.nav-item');
            if (navItem) {
                navItem.classList.add('active');
            }
        }
    });
});