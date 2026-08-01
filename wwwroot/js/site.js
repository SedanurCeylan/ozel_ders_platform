(()=>{
  const menu=document.querySelector('.site-nav .navbar-collapse');
  if(!menu)return;

  menu.addEventListener('click',event=>{
    const link=event.target.closest('a');
    if(!link||link.classList.contains('dropdown-toggle')||!menu.classList.contains('show'))return;
    const collapse=bootstrap.Collapse.getOrCreateInstance(menu,{toggle:false});
    collapse.hide();
  });
})();
