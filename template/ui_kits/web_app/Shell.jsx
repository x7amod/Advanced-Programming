/* global React, I, Btn, Avatar */
const { useState } = React;

const NAV = {
  Trainee: [
    { key:'dashboard', label:'Dashboard', icon: I.home },
    { key:'browse', label:'Browse courses', icon: I.book },
    { key:'enrollments', label:'My enrollments', icon: I.cal },
    { key:'certifications', label:'My certifications', icon: I.award },
    { key:'payments', label:'My payments', icon: I.card },
    { key:'results', label:'My results', icon: I.clip },
    { key:'notifications', label:'Notifications', icon: I.bell },
    { key:'profile', label:'My profile', icon: I.user },
  ],
  Instructor: [
    { key:'dashboard', label:'Dashboard', icon: I.home },
    { key:'sessions', label:'My sessions', icon: I.cal },
    { key:'assess', label:'Record assessment', icon: I.clip },
    { key:'notifications', label:'Notifications', icon: I.bell },
    { key:'profile', label:'My profile', icon: I.user },
  ],
  Coordinator: [
    { key:'dashboard', label:'Dashboard', icon: I.home },
    { key:'users', label:'Users', icon: I.users },
    { key:'courses', label:'Courses', icon: I.book },
    { key:'classrooms', label:'Classrooms', icon: I.home },
    { key:'sessions', label:'Sessions', icon: I.cal },
    { key:'enrollments', label:'Enrollments', icon: I.clip },
    { key:'payments', label:'Payments', icon: I.card },
    { key:'tracks', label:'Certification tracks', icon: I.award },
    { key:'reports', label:'Reports', icon: I.bar },
    { key:'notifications', label:'Notifications', icon: I.bell },
    { key:'profile', label:'My profile', icon: I.user },
  ],
};

const Sidebar = ({ role, screen, setScreen, onLogout }) => (
  <aside style={{
    width:240, flex:'none', background:'#fff', borderRight:'1px solid #E5E7EB',
    display:'flex', flexDirection:'column', height:'100vh', position:'sticky', top:0
  }}>
    <div style={{display:'flex', alignItems:'center', gap:10, padding:'16px 18px', borderBottom:'1px solid #EEF0F2', height:64}}>
      <img src="../../assets/logo.svg" alt="Taalam" style={{height:24}}/>
    </div>
    <div style={{padding:'10px 10px 4px', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', color:'#9CA3AF'}}>
      {role}
    </div>
    <nav style={{padding:'4px 10px', display:'flex', flexDirection:'column', gap:2, flex:1, overflow:'auto'}}>
      {NAV[role].map(item => {
        const active = screen === item.key;
        return (
          <button key={item.key} onClick={() => setScreen(item.key)} style={{
            display:'flex', alignItems:'center', gap:10, padding:'8px 10px',
            border:0, background: active ? 'rgba(13,148,136,0.08)' : 'transparent',
            color: active ? '#0D9488' : '#6B7280',
            font: active ? '600 14px/22px Inter, sans-serif' : '500 14px/22px Inter, sans-serif',
            borderRadius:8, cursor:'pointer', textAlign:'left', width:'100%'
          }}
          onMouseEnter={e => { if(!active){ e.currentTarget.style.background='#F3F4F6'; e.currentTarget.style.color='#1A1A2E'; }}}
          onMouseLeave={e => { if(!active){ e.currentTarget.style.background='transparent'; e.currentTarget.style.color='#6B7280'; }}}
          >
            <span style={{display:'inline-flex'}}>{item.icon}</span>
            {item.label}
          </button>
        );
      })}
    </nav>
    <div style={{padding:'10px', borderTop:'1px solid #EEF0F2'}}>
      <button onClick={onLogout} style={{
        display:'flex', alignItems:'center', gap:10, padding:'8px 10px',
        border:0, background:'transparent', color:'#6B7280', width:'100%',
        font:'500 14px/22px Inter, sans-serif', borderRadius:8, cursor:'pointer', textAlign:'left'
      }}
      onMouseEnter={e => { e.currentTarget.style.background='#F3F4F6'; e.currentTarget.style.color='#991B1B'; }}
      onMouseLeave={e => { e.currentTarget.style.background='transparent'; e.currentTarget.style.color='#6B7280'; }}
      >
        {I.logout} Sign out
      </button>
    </div>
  </aside>
);

const TopBar = ({ title, user, onProfile }) => (
  <header style={{
    height:64, background:'rgba(255,255,255,0.92)', backdropFilter:'blur(8px)',
    borderBottom:'1px solid #E5E7EB', position:'sticky', top:0, zIndex:10,
    display:'flex', alignItems:'center', padding:'0 32px', gap:24
  }}>
    <div style={{font:'600 20px/28px Inter, sans-serif', color:'#1A1A2E', flex:1, letterSpacing:'-0.005em'}}>{title}</div>
    <div style={{position:'relative', width:280}}>
      <span style={{position:'absolute', left:10, top:'50%', transform:'translateY(-50%)', color:'#9CA3AF'}}>{I.search}</span>
      <input placeholder="Search courses, sessions, trainees…" style={{
        width:'100%', padding:'8px 12px 8px 36px', borderRadius:8, border:'1px solid #E5E7EB',
        font:'400 13px/20px Inter, sans-serif', background:'#F8F9FA', color:'#1A1A2E', outline:'none'
      }}/>
    </div>
    <button title="Notifications" style={{
      position:'relative', background:'#F3F4F6', border:0, borderRadius:8, width:36, height:36,
      display:'inline-flex', alignItems:'center', justifyContent:'center', color:'#6B7280', cursor:'pointer'
    }}>
      {I.bell}
      <span style={{position:'absolute', top:6, right:6, width:8, height:8, borderRadius:999, background:'#EF4444', boxShadow:'0 0 0 2px #F3F4F6'}}/>
    </button>
    <button onClick={onProfile} style={{display:'flex', alignItems:'center', gap:10, background:'transparent', border:0, cursor:'pointer'}}>
      <Avatar name={user.name}/>
      <div style={{textAlign:'left'}}>
        <div style={{font:'600 13px/16px Inter, sans-serif', color:'#1A1A2E'}}>{user.name}</div>
        <div style={{font:'400 12px/14px Inter, sans-serif', color:'#6B7280'}}>{user.role}</div>
      </div>
    </button>
  </header>
);

const Shell = ({ role, user, screen, setScreen, onLogout, title, children }) => (
  <div style={{display:'flex', minHeight:'100vh', background:'#F8F9FA'}}>
    <Sidebar role={role} screen={screen} setScreen={setScreen} onLogout={onLogout}/>
    <div style={{flex:1, minWidth:0, display:'flex', flexDirection:'column'}}>
      <TopBar title={title} user={user}/>
      <main style={{padding:'32px', maxWidth:1280, width:'100%', margin:'0 auto'}}>{children}</main>
    </div>
  </div>
);

Object.assign(window, { Shell, NAV });
