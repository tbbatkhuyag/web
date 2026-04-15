function refreshDynamicMenu() {
    fetch('/api/editor/files')
    .then(r => r.json())
    .then(treeData => {
        function filterFiles(nodes) {
            return nodes.filter(n => {
                if(n.id === 'manager.html' || n.id === 'grapesjs.html' || n.id === 'inject.ps1') return false;
                if(n.children) n.children = filterFiles(n.children);
                return true;
            });
        }
        treeData = filterFiles(treeData);

        function formatLabel(name) {
            if(name.toLowerCase() === 'index.html') return 'Home';
            return name.replace(/\.html$/, '').replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
        }

        function buildDesktopMenu(nodes) {
            let html = '';
            nodes.forEach(n => {
                const hasChildren = n.children && n.children.length > 0;
                const active = location.pathname.endsWith(n.id) || (location.pathname.endsWith('/') && n.id === 'index.html') ? 'current-menu-item' : '';
                const liClass = `menu-item ${hasChildren?'menu-item-has-children':''} ${active} kingster-normal-menu`;
                const aClass = hasChildren ? 'sf-with-ul-pre' : '';
                html += `<li class="${liClass}"><a href="${n.id}" class="${aClass}">${formatLabel(n.text)}</a>`;
                if(hasChildren) {
                    html += `<ul class="sub-menu">` + buildDesktopMenu(n.children) + `</ul>`;
                }
                html += `</li>`;
            });
            return html;
        }

        function buildMobileMenu(nodes) {
            let html = '';
            nodes.forEach(n => {
                const hasChildren = n.children && n.children.length > 0;
                const active = location.pathname.endsWith(n.id) || (location.pathname.endsWith('/') && n.id === 'index.html') ? 'current-menu-item' : '';
                const liClass = `menu-item ${hasChildren?'menu-item-has-children':''} ${active}`;
                html += `<li class="${liClass}"><a href="${n.id}">${formatLabel(n.text)}</a>`;
                if(hasChildren) {
                    html += `<ul class="sub-menu">` + buildMobileMenu(n.children) + `</ul>`;
                }
                html += `</li>`;
            });
            return html;
        }

        const dkMenu = document.getElementById("menu-main-navigation-1");
        if(dkMenu) dkMenu.innerHTML = buildDesktopMenu(treeData);

        const mbMenu = document.getElementById("menu-main-navigation");
        if(mbMenu) mbMenu.innerHTML = buildMobileMenu(treeData);
    })
    .catch(err => console.error('Menu load error:', err));
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", refreshDynamicMenu);
} else {
    refreshDynamicMenu();
}
window.refreshDynamicMenu = refreshDynamicMenu;
