(() => {
  const button = document.querySelector('[data-sidebar-toggle]');
  const mobile = () => matchMedia('(max-width:760px)').matches;
  if (!button) return;
  const current = location.pathname.replace(/\/$/, '').toLowerCase() + location.search.toLowerCase();
  let best = null;
  document.querySelectorAll('.admin-sidebar nav a[href]').forEach(link => {
    const url = new URL(link.href, location.origin);
    if (url.origin !== location.origin) return;
    const target = url.pathname.replace(/\/$/, '').toLowerCase() + url.search.toLowerCase();
    if (target && current === target && (!best || target.length > best.target.length)) best = { link, target };
  });
  const groups = [...document.querySelectorAll('.nav-group')];
  if (best) { best.link.classList.add('active'); const group = best.link.closest('details'); if (group) group.open = true; }
  groups.forEach(group => group.addEventListener('toggle', () => {
    if (group.open) groups.forEach(other => { if (other !== group) other.open = false; });
  }));
  const update = () => {
    const open = mobile() ? document.body.classList.contains('sidebar-open') : !document.body.classList.contains('sidebar-collapsed');
    button.setAttribute('aria-expanded', String(open)); button.textContent = open ? '✕' : '☰';
  };
  if (!mobile() && localStorage.getItem('admin-sidebar-collapsed') === 'true') document.body.classList.add('sidebar-collapsed');
  button.addEventListener('click', () => {
    if (mobile()) document.body.classList.toggle('sidebar-open');
    else { document.body.classList.toggle('sidebar-collapsed'); localStorage.setItem('admin-sidebar-collapsed', String(document.body.classList.contains('sidebar-collapsed'))); }
    update();
  });
  document.querySelectorAll('.admin-sidebar nav a').forEach(link => link.addEventListener('click', () => { if (mobile()) document.body.classList.remove('sidebar-open'); }));
  addEventListener('resize', update); update();
})();
