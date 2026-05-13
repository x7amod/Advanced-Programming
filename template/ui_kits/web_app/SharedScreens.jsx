/* global React, I, Btn, Badge, Card, Avatar */
const { useState } = React;

// =================== My Results (Trainee) ===================
const MyResults = () => {
  const [filter, setFilter] = useState('All');
  const all = [
    { code:'PMP-100', course:'Project Management Fundamentals', date:'12 May 2026', instructor:'Fatima Khalifa', result:'Pass', remarks:'Strong grasp of project lifecycle.', adate:'14 May 2026' },
    { code:'OSH-100', course:'Occupational Safety, Level 1', date:'14 Mar 2026', instructor:'Omar Saeed', result:'Pass', remarks:'Excellent practical demonstration.', adate:'16 Mar 2026' },
    { code:'EXC-101', course:'Excel for Analysts', date:'02 Feb 2026', instructor:'Layla Mansour', result:'Fail', remarks:'Recommend retake; missed pivot tables section.', adate:'04 Feb 2026' },
    { code:'AML-110', course:'Anti-Money Laundering Essentials', date:'14 Mar 2026', instructor:'Fatima Khalifa', result:'Pass', remarks:'Well-prepared. Clear understanding of BHB regs.', adate:'16 Mar 2026' },
  ];
  const rows = filter==='All' ? all : all.filter(r => r.result===filter);
  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-end', marginBottom:20}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My results</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Assessment outcomes recorded by your instructors.</div>
        </div>
        <div style={{display:'flex', gap:6, padding:4, background:'#F3F4F6', borderRadius:8}}>
          {['All','Pass','Fail'].map(f => (
            <button key={f} onClick={()=>setFilter(f)} style={{
              padding:'6px 14px', border:0, borderRadius:6, cursor:'pointer',
              background: filter===f ? '#fff' : 'transparent',
              color: filter===f ? '#0D9488' : '#6B7280',
              font: filter===f ? '600 13px/18px Inter, sans-serif' : '500 13px/18px Inter, sans-serif',
              boxShadow: filter===f ? '0 1px 2px rgba(0,0,0,0.06)' : 'none'
            }}>{f}</button>
          ))}
        </div>
      </div>
      {rows.length===0 ? (
        <Card>
          <div style={{padding:'48px 20px', textAlign:'center'}}>
            <div style={{width:56, height:56, borderRadius:999, background:'#F3F4F6', color:'#9CA3AF', display:'inline-flex', alignItems:'center', justifyContent:'center', marginBottom:14}}>{I.clip}</div>
            <div style={{font:'600 16px/24px Inter, sans-serif', color:'#1A1A2E'}}>No results yet</div>
            <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Once your instructors record assessments, they'll appear here.</div>
          </div>
        </Card>
      ) : (
        <Card padded={false}>
          <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
            <thead><tr>{['Course','Session date','Instructor','Result','Remarks','Assessment date'].map((h,i)=>(
              <th key={i} style={{textAlign:'left', padding:'10px 16px', background:'#F3F4F6', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em', color:'#6B7280', borderBottom:'1px solid #E5E7EB'}}>{h}</th>
            ))}</tr></thead>
            <tbody>{rows.map((r,i)=>(
              <tr key={i} onMouseEnter={e=>e.currentTarget.style.background='#F8F9FA'} onMouseLeave={e=>e.currentTarget.style.background='transparent'}>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><div style={{fontWeight:500}}>{r.course}</div><div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280'}}>{r.code}</div></td>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', fontVariantNumeric:'tabular-nums', color:'#1A1A2E'}}>{r.date}</td>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E'}}>{r.instructor}</td>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status={r.result}/></td>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280', maxWidth:280}}>{r.remarks}</td>
                <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', fontVariantNumeric:'tabular-nums', color:'#1A1A2E'}}>{r.adate}</td>
              </tr>
            ))}</tbody>
          </table>
        </Card>
      )}
    </>
  );
};

// =================== Notifications ===================
const NOTIFS_BY_ROLE = {
  Trainee: [
    { type:'Enrollment', icon:I.cal, color:'#0D9488', bg:'#CCFBF1', title:'Enrollment confirmed', msg:'You\'re enrolled in PMP-100 · session 12 May 2026.', time:'2 hours ago', unread:true },
    { type:'Payment', icon:I.card, color:'#F59E0B', bg:'#FEF3C7', title:'Invoice due soon', msg:'INV-2026-0142 (BHD 85.000) is due 20 May 2026.', time:'5 hours ago', unread:true },
    { type:'Assessment', icon:I.clip, color:'#3B82F6', bg:'#DBEAFE', title:'New result recorded', msg:'Your AML-110 result is available — Pass.', time:'1 day ago', unread:false },
    { type:'Certification', icon:I.award, color:'#10B981', bg:'#D1FAE5', title:'Certification awarded', msg:'You\'ve earned: Occupational Safety, Level 1.', time:'3 days ago', unread:false },
  ],
  Instructor: [
    { type:'Assessment', icon:I.clip, color:'#3B82F6', bg:'#DBEAFE', title:'Assessment overdue', msg:'12 trainees in PMP-100 still need assessment.', time:'1 hour ago', unread:true },
    { type:'Enrollment', icon:I.cal, color:'#0D9488', bg:'#CCFBF1', title:'New session assigned', msg:'You\'ve been assigned to OSH-200 on 18 Jun.', time:'4 hours ago', unread:true },
    { type:'Enrollment', icon:I.users, color:'#0D9488', bg:'#CCFBF1', title:'Trainee enrolled', msg:'Omar Saeed joined your AML-110 session.', time:'1 day ago', unread:false },
  ],
  Coordinator: [
    { type:'Payment', icon:I.card, color:'#EF4444', bg:'#FEE2E2', title:'5 invoices overdue', msg:'BHD 1,840 in invoices > 30 days past due.', time:'30 min ago', unread:true },
    { type:'Certification', icon:I.award, color:'#10B981', bg:'#D1FAE5', title:'7 certifications ready', msg:'Trainees have completed all required courses.', time:'2 hours ago', unread:true },
    { type:'Enrollment', icon:I.users, color:'#0D9488', bg:'#CCFBF1', title:'Waitlist threshold', msg:'OSH-200 has 12 trainees waitlisted.', time:'6 hours ago', unread:true },
    { type:'Assessment', icon:I.clip, color:'#3B82F6', bg:'#DBEAFE', title:'Assessments awaiting approval', msg:'4 assessments from Fatima Khalifa need review.', time:'1 day ago', unread:false },
  ],
};

const Notifications = ({ role }) => {
  const [tab, setTab] = useState('All');
  const [items, setItems] = useState(NOTIFS_BY_ROLE[role] || []);
  const tabs = ['All','Unread','Enrollment','Assessment','Payment','Certification'];
  const counts = { All:items.length, Unread:items.filter(n=>n.unread).length };
  const filtered = items.filter(n =>
    tab==='All' ? true : tab==='Unread' ? n.unread : n.type===tab
  );
  const markAll = () => setItems(items.map(n => ({...n, unread:false})));
  const markOne = (i) => setItems(items.map((n,j)=> j===i ? {...n, unread:false} : n));

  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-end', marginBottom:18}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Notifications</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>{counts.Unread} unread of {counts.All} total.</div>
        </div>
        <Btn variant="secondary" size="sm" icon={I.check} onClick={markAll}>Mark all as read</Btn>
      </div>
      <div style={{display:'flex', gap:6, marginBottom:14, flexWrap:'wrap'}}>
        {tabs.map(t => {
          const active = tab===t;
          const n = t==='All'?counts.All : t==='Unread'?counts.Unread : items.filter(x=>x.type===t).length;
          return (
            <button key={t} onClick={()=>setTab(t)} style={{
              padding:'6px 12px', borderRadius:8,
              border: active?'1px solid #0D9488':'1px solid #E5E7EB',
              background: active?'rgba(13,148,136,0.08)':'#fff',
              color: active?'#0F766E':'#6B7280',
              font: active?'600 13px/18px Inter, sans-serif':'500 13px/18px Inter, sans-serif',
              cursor:'pointer', display:'inline-flex', alignItems:'center', gap:8
            }}>{t}<span style={{fontVariantNumeric:'tabular-nums', color:active?'#0F766E':'#9CA3AF'}}>{n}</span></button>
          );
        })}
      </div>
      {filtered.length===0 ? (
        <Card>
          <div style={{padding:'48px 20px', textAlign:'center'}}>
            <div style={{width:56, height:56, borderRadius:999, background:'#F3F4F6', color:'#9CA3AF', display:'inline-flex', alignItems:'center', justifyContent:'center', marginBottom:14}}>{I.bell}</div>
            <div style={{font:'600 16px/24px Inter, sans-serif', color:'#1A1A2E'}}>You're all caught up</div>
            <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>No notifications match this filter.</div>
          </div>
        </Card>
      ) : (
        <Card padded={false}>
          <div style={{display:'flex', flexDirection:'column'}}>
            {filtered.map((n,i)=>{
              const idx = items.indexOf(n);
              return (
                <div key={i} style={{display:'flex', alignItems:'flex-start', gap:14, padding:'16px 20px', borderBottom: i===filtered.length-1?0:'1px solid #EEF0F2', background:n.unread?'rgba(13,148,136,0.03)':'transparent', position:'relative'}}>
                  {n.unread && <div style={{position:'absolute', left:8, top:'50%', transform:'translateY(-50%)', width:6, height:6, borderRadius:999, background:'#0D9488'}}/>}
                  <div style={{width:38, height:38, borderRadius:999, background:n.bg, color:n.color, display:'flex', alignItems:'center', justifyContent:'center', flex:'none'}}>{n.icon}</div>
                  <div style={{flex:1, minWidth:0}}>
                    <div style={{display:'flex', justifyContent:'space-between', alignItems:'baseline', gap:10}}>
                      <div style={{font:'600 14px/20px Inter, sans-serif', color:'#1A1A2E'}}>{n.title}</div>
                      <div style={{font:'400 12px/16px Inter, sans-serif', color:'#9CA3AF', flex:'none'}}>{n.time}</div>
                    </div>
                    <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:2}}>{n.msg}</div>
                  </div>
                  {n.unread && <button onClick={()=>markOne(idx)} style={{background:'transparent', border:0, color:'#0D9488', font:'500 12px/16px Inter, sans-serif', cursor:'pointer', flex:'none', padding:'4px 8px'}}>Mark as read</button>}
                </div>
              );
            })}
          </div>
        </Card>
      )}
    </>
  );
};

// =================== My Profile ===================
const Profile = ({ role }) => {
  const base = { name:'Abdulla Alansari', email:'user@taalam.bh', phone:'+973 3300 5678', joined:'14 Jan 2026' };
  return (
    <>
      <div style={{marginBottom:20}}>
        <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My profile</div>
        <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Personal information and account preferences.</div>
      </div>
      <div style={{display:'grid', gridTemplateColumns:'1fr 1.5fr', gap:16, marginBottom:16}}>
        <Card>
          <div style={{display:'flex', flexDirection:'column', alignItems:'center', textAlign:'center', paddingBottom:16, borderBottom:'1px solid #EEF0F2'}}>
            <Avatar name={base.name} size={84}/>
            <div style={{font:'700 18px/24px Inter, sans-serif', color:'#1A1A2E', marginTop:14}}>{base.name}</div>
            <div style={{marginTop:6}}><Badge status={role} dot={false}/></div>
          </div>
          <div style={{display:'flex', flexDirection:'column', gap:12, padding:'16px 0', font:'400 13px/20px Inter, sans-serif'}}>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Email</span><span style={{color:'#1A1A2E', fontWeight:500}}>{base.email}</span></div>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Phone</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{base.phone}</span></div>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Joined</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{base.joined}</span></div>
          </div>
          <Btn full variant="secondary" icon={I.edit}>Edit profile</Btn>
        </Card>
        <Card title={role==='Trainee'?'Trainee details':role==='Instructor'?'Instructor details':'Coordinator details'}>
          {role==='Trainee' && (
            <div style={{display:'flex', flexDirection:'column', gap:14}}>
              <Row label="Date of birth" value="14 Feb 1995"/>
              <Row label="Address" value="Building 142, Road 2832, Block 428, Manama, Bahrain"/>
              <Row label="Emergency contact" value="Sarah Alansari · +973 3300 9999 (sister)"/>
            </div>
          )}
          {role==='Instructor' && (
            <div style={{display:'flex', flexDirection:'column', gap:14}}>
              <Row label="Hire date" value="02 Sep 2024"/>
              <Row label="Bio" value="Senior trainer with 10+ years in IT service management and project delivery. Certified PMP, ITIL Expert, and AgilePM Practitioner."/>
              <div>
                <div style={{font:'600 12px/16px Inter, sans-serif', color:'#6B7280', textTransform:'uppercase', letterSpacing:'0.04em', marginBottom:8}}>Subject expertise</div>
                <div style={{display:'flex', flexWrap:'wrap', gap:6}}>
                  {['Project Management','IT Service Management','Agile','Risk Management','Stakeholder Comms'].map(t => (
                    <span key={t} style={{padding:'4px 10px', borderRadius:9999, background:'#F3F4F6', color:'#1A1A2E', font:'500 12px/16px Inter, sans-serif'}}>{t}</span>
                  ))}
                </div>
              </div>
            </div>
          )}
          {role==='Coordinator' && (
            <div style={{display:'flex', flexDirection:'column', gap:14}}>
              <Row label="Department" value="Training Operations"/>
              <Row label="Office" value="Headquarters · Manama"/>
              <Row label="Reports to" value="Director of Operations"/>
            </div>
          )}
        </Card>
      </div>
      <Card title="Change password">
        <div style={{display:'grid', gridTemplateColumns:'1fr 1fr 1fr', gap:14}}>
          {['Current password','New password','Confirm new password'].map(l => (
            <div key={l}>
              <label style={{display:'block', font:'600 12px/16px Inter, sans-serif', color:'#1A1A2E', marginBottom:6}}>{l}</label>
              <input type="password" defaultValue="••••••••••" style={{width:'100%', padding:'9px 12px', border:'1px solid #E5E7EB', borderRadius:4, font:'400 14px/22px Inter, sans-serif', background:'#fff', outline:'none'}}/>
            </div>
          ))}
        </div>
        <div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280', marginTop:10}}>Use at least 8 characters with one number.</div>
        <div style={{display:'flex', justifyContent:'flex-end', marginTop:14}}>
          <Btn>Save changes</Btn>
        </div>
      </Card>
    </>
  );
};
const Row = ({ label, value }) => (
  <div>
    <div style={{font:'600 12px/16px Inter, sans-serif', color:'#6B7280', textTransform:'uppercase', letterSpacing:'0.04em', marginBottom:4}}>{label}</div>
    <div style={{font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E'}}>{value}</div>
  </div>
);

Object.assign(window, { MyResults, Notifications, Profile });
