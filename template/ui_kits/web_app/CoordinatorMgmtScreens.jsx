/* global React, I, Btn, Badge, Card, Stat, Field, Input, Select, Avatar */
const { useState } = React;

// =================== Course Management ===================
const CourseMgmt = () => {
  const [open, setOpen] = useState(false);
  const rows = [
    { code:'PMP-100', title:'Project Management Fundamentals', cat:'Professional', area:'Business', hrs:24, fee:120, cap:20, status:'Active', pre:'—' },
    { code:'OSH-200', title:'Occupational Safety, Level 2', cat:'Compliance', area:'Safety', hrs:16, fee:85, cap:25, status:'Active', pre:'OSH-100' },
    { code:'ITIL-300', title:'ITIL 4 Foundation', cat:'Certification', area:'IT', hrs:32, fee:240, cap:18, status:'Active', pre:'—' },
    { code:'AML-110', title:'Anti-Money Laundering Essentials', cat:'Compliance', area:'Finance', hrs:12, fee:95, cap:30, status:'Active', pre:'—' },
    { code:'EXC-101', title:'Excel for Analysts', cat:'Skills', area:'IT', hrs:18, fee:60, cap:18, status:'Inactive', pre:'—' },
  ];
  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:18}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Courses</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Catalog of courses offered by Taalam.</div>
        </div>
        <Btn icon={I.plus} onClick={()=>setOpen(true)}>Add course</Btn>
      </div>
      <div style={{display:'flex', gap:12, marginBottom:16}}>
        <div style={{position:'relative', flex:'1 1 280px', minWidth:240}}>
          <span style={{position:'absolute', left:12, top:'50%', transform:'translateY(-50%)', color:'#9CA3AF'}}>{I.search}</span>
          <input placeholder="Search course code or title…" style={{width:'100%', padding:'10px 12px 10px 38px', borderRadius:8, border:'1px solid #E5E7EB', font:'400 14px/22px Inter, sans-serif', background:'#fff'}}/>
        </div>
        <Select style={{width:180}}><option>All categories</option><option>Professional</option><option>Compliance</option><option>Certification</option><option>Skills</option></Select>
        <Select style={{width:180}}><option>All subject areas</option></Select>
        <Select style={{width:140}}><option>All statuses</option><option>Active</option><option>Inactive</option></Select>
      </div>
      <Card padded={false}>
        <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
          <thead><tr>{['Code','Title','Category','Area','Duration','Fee (BHD)','Capacity','Status','Prerequisites',''].map((h,i)=>(
            <th key={i} style={{textAlign:'left', padding:'10px 16px', background:'#F3F4F6', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em', color:'#6B7280', borderBottom:'1px solid #E5E7EB'}}>{h}</th>
          ))}</tr></thead>
          <tbody>{rows.map((r,i)=>(
            <tr key={i} onMouseEnter={e=>e.currentTarget.style.background='#F8F9FA'} onMouseLeave={e=>e.currentTarget.style.background='transparent'}>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', font:'500 13px/18px SFMono-Regular, ui-monospace, monospace', color:'#0F766E'}}>{r.code}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontWeight:500}}>{r.title}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280'}}>{r.cat}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280'}}>{r.area}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.hrs} hrs</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.fee.toFixed(3)}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.cap}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status={r.status}/></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280', font:'500 13px/18px SFMono-Regular, ui-monospace, monospace'}}>{r.pre}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', textAlign:'right'}}><div style={{display:'inline-flex', gap:6, color:'#6B7280'}}><button onClick={()=>setOpen(true)} style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.edit}</button><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.trash}</button></div></td>
            </tr>
          ))}</tbody>
        </table>
      </Card>
      {open && <CourseFormModal onClose={()=>setOpen(false)}/>}
    </>
  );
};

const CourseFormModal = ({ onClose }) => (
  <div style={{position:'fixed', inset:0, background:'rgba(15,23,42,0.4)', display:'flex', alignItems:'center', justifyContent:'center', zIndex:50, padding:24}} onClick={onClose}>
    <div onClick={e=>e.stopPropagation()} style={{background:'#fff', borderRadius:12, width:'100%', maxWidth:680, maxHeight:'90vh', overflow:'auto', boxShadow:'0 20px 50px rgba(0,0,0,0.2)'}}>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', padding:'18px 24px', borderBottom:'1px solid #EEF0F2'}}>
        <div style={{font:'700 18px/24px Inter, sans-serif', color:'#1A1A2E'}}>Add course</div>
        <button onClick={onClose} style={{background:'transparent', border:0, color:'#6B7280', cursor:'pointer'}}>{I.x}</button>
      </div>
      <div style={{padding:24, display:'grid', gridTemplateColumns:'1fr 1fr', gap:14}}>
        <Field label="Course code"><Input placeholder="e.g. PMP-200"/></Field>
        <Field label="Title"><Input placeholder="Course title"/></Field>
        <div style={{gridColumn:'1 / -1'}}><Field label="Description"><textarea placeholder="Short description shown to trainees" style={{width:'100%', minHeight:96, padding:'10px 12px', border:'1px solid #E5E7EB', borderRadius:4, font:'400 14px/22px Inter, sans-serif', resize:'vertical'}}/></Field></div>
        <Field label="Category"><Select><option>Professional</option><option>Compliance</option><option>Certification</option><option>Skills</option></Select></Field>
        <Field label="Subject area"><Select><option>Business</option><option>IT</option><option>Safety</option><option>Finance</option></Select></Field>
        <Field label="Duration (hours)"><Input type="number" defaultValue="24"/></Field>
        <Field label="Max capacity"><Input type="number" defaultValue="20"/></Field>
        <Field label="Enrollment fee (BHD)"><Input type="number" defaultValue="120.000"/></Field>
        <Field label="Prerequisite course"><Select><option>None</option><option>PMP-100</option><option>OSH-100</option></Select></Field>
        <div style={{gridColumn:'1 / -1'}}><Field label="Equipment requirements"><Input placeholder="e.g. Laptop with Excel; Projector"/></Field></div>
        <div style={{gridColumn:'1 / -1', display:'flex', alignItems:'center', gap:10, padding:'10px 0'}}>
          <label style={{display:'inline-flex', alignItems:'center', gap:8, cursor:'pointer'}}>
            <input type="checkbox" defaultChecked style={{accentColor:'#0D9488', width:14, height:14}}/>
            <span style={{font:'500 14px/22px Inter, sans-serif', color:'#1A1A2E'}}>Active — visible in catalog and open for enrollment</span>
          </label>
        </div>
      </div>
      <div style={{display:'flex', justifyContent:'flex-end', gap:10, padding:'16px 24px', borderTop:'1px solid #EEF0F2'}}>
        <Btn variant="ghost" onClick={onClose}>Cancel</Btn>
        <Btn onClick={onClose}>Save course</Btn>
      </div>
    </div>
  </div>
);

// =================== Enrollment Management ===================
const EnrollmentMgmt = () => {
  const rows = [
    { name:'Layla Mansour', course:'PMP-100', session:'12 May 2026', enr:'06 May 2026', s:'Enrolled', fs:'Paid' },
    { name:'Omar Saeed', course:'OSH-200', session:'20 May 2026', enr:'02 May 2026', s:'Waitlisted', fs:'Pending' },
    { name:'Yousef Rashid', course:'ITIL-300', session:'04 Jun 2026', enr:'28 Apr 2026', s:'Enrolled', fs:'Partially paid' },
    { name:'Fatima Khalifa', course:'AML-110', session:'18 Jun 2026', enr:'25 Apr 2026', s:'Enrolled', fs:'Paid' },
    { name:'Omar Saeed', course:'EXC-101', session:'02 Feb 2026', enr:'20 Jan 2026', s:'Dropped', fs:'Refunded' },
    { name:'Layla Mansour', course:'OSH-100', session:'14 Mar 2026', enr:'01 Mar 2026', s:'Completed', fs:'Paid' },
  ];
  return (
    <>
      <div style={{marginBottom:18}}>
        <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Enrollments</div>
        <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Trainee enrollments across all sessions.</div>
      </div>
      <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:16, marginBottom:16}}>
        <Stat label="Total enrollments" value="401" hint="last 90 days" icon={I.clip}/>
        <Stat label="Active" value="284" hint="enrolled or waitlisted" icon={I.users}/>
        <Stat label="Completed" value="98" hint="this quarter" trend="up" icon={I.check}/>
        <Stat label="Dropped" value="19" hint="4.7% drop rate" trend="down" icon={I.x}/>
      </div>
      <div style={{display:'flex', gap:12, marginBottom:16}}>
        <div style={{position:'relative', flex:'1 1 280px', minWidth:240}}>
          <span style={{position:'absolute', left:12, top:'50%', transform:'translateY(-50%)', color:'#9CA3AF'}}>{I.search}</span>
          <input placeholder="Search trainee or course…" style={{width:'100%', padding:'10px 12px 10px 38px', borderRadius:8, border:'1px solid #E5E7EB', font:'400 14px/22px Inter, sans-serif', background:'#fff'}}/>
        </div>
        <Select style={{width:180}}><option>All statuses</option><option>Enrolled</option><option>Waitlisted</option><option>Completed</option><option>Dropped</option></Select>
        <Select style={{width:200}}><option>All sessions</option></Select>
      </div>
      <Card padded={false}>
        <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
          <thead><tr>{['Trainee','Course','Session date','Enrolled on','Status','Fee status',''].map((h,i)=>(
            <th key={i} style={{textAlign:'left', padding:'10px 16px', background:'#F3F4F6', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em', color:'#6B7280', borderBottom:'1px solid #E5E7EB'}}>{h}</th>
          ))}</tr></thead>
          <tbody>{rows.map((r,i)=>(
            <tr key={i} onMouseEnter={e=>e.currentTarget.style.background='#F8F9FA'} onMouseLeave={e=>e.currentTarget.style.background='transparent'}>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><div style={{display:'flex', alignItems:'center', gap:10}}><Avatar name={r.name} size={28}/><span style={{fontWeight:500, color:'#1A1A2E'}}>{r.name}</span></div></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E'}}>{r.course}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.session}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280', fontVariantNumeric:'tabular-nums'}}>{r.enr}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status={r.s}/></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status={r.fs}/></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', textAlign:'right'}}><Btn size="sm" variant="ghost">View</Btn></td>
            </tr>
          ))}</tbody>
        </table>
      </Card>
    </>
  );
};

// =================== Certification Track Management ===================
const TrackMgmt = () => {
  const rows = [
    { name:'IT Service Management', desc:'ITIL-aligned 5-course path', req:5, active:42, comp:68 },
    { name:'Project Management Track', desc:'PMP-aligned 4-course path', req:4, active:84, comp:74 },
    { name:'Workplace Safety Certification', desc:'OSH levels 1–3', req:3, active:56, comp:81 },
    { name:'Financial Compliance Track', desc:'AML, KYC, regulatory reporting', req:4, active:28, comp:62 },
  ];
  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:18}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Certification tracks</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Multi-course paths leading to a certification.</div>
        </div>
        <Btn icon={I.plus}>Add track</Btn>
      </div>
      <Card padded={false}>
        <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
          <thead><tr>{['Track','Required courses','Active trainees','Completion rate','Status',''].map((h,i)=>(
            <th key={i} style={{textAlign:'left', padding:'10px 16px', background:'#F3F4F6', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em', color:'#6B7280', borderBottom:'1px solid #E5E7EB'}}>{h}</th>
          ))}</tr></thead>
          <tbody>{rows.map((r,i)=>(
            <tr key={i} onMouseEnter={e=>e.currentTarget.style.background='#F8F9FA'} onMouseLeave={e=>e.currentTarget.style.background='transparent'} style={{cursor:'pointer'}}>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><div style={{display:'flex', alignItems:'center', gap:10}}><div style={{width:36, height:36, borderRadius:8, background:'#CCFBF1', color:'#0F766E', display:'flex', alignItems:'center', justifyContent:'center'}}>{I.award}</div><div><div style={{fontWeight:500, color:'#1A1A2E'}}>{r.name}</div><div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280'}}>{r.desc}</div></div></div></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.req} courses</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.active}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><div style={{display:'flex', alignItems:'center', gap:10}}><div style={{width:80, height:6, background:'#F3F4F6', borderRadius:99}}><div style={{width:`${r.comp}%`, height:'100%', background:'#0D9488', borderRadius:99}}/></div><span style={{font:'500 13px/18px Inter, sans-serif', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.comp}%</span></div></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status="Active"/></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', textAlign:'right'}}><div style={{display:'inline-flex', gap:6, color:'#6B7280'}}><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.edit}</button><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.trash}</button></div></td>
            </tr>
          ))}</tbody>
        </table>
      </Card>
    </>
  );
};

// =================== Classroom Management ===================
const ClassroomMgmt = () => {
  const rows = [
    { name:'Room 204', building:'HQ', floor:2, loc:'Manama', cap:24, eq:6, s:'Active' },
    { name:'Lab A', building:'HQ', floor:1, loc:'Manama', cap:18, eq:12, s:'Active' },
    { name:'Lab B', building:'Annex', floor:1, loc:'Manama', cap:18, eq:12, s:'Active' },
    { name:'Room 110', building:'HQ', floor:1, loc:'Manama', cap:30, eq:4, s:'Active' },
    { name:'Auditorium', building:'HQ', floor:0, loc:'Manama', cap:80, eq:8, s:'Inactive' },
  ];
  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:18}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Classrooms</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Physical spaces and equipment available for sessions.</div>
        </div>
        <Btn icon={I.plus}>Add classroom</Btn>
      </div>
      <Card padded={false}>
        <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
          <thead><tr>{['Name','Building','Floor','Location','Capacity','Equipment','Status',''].map((h,i)=>(
            <th key={i} style={{textAlign:'left', padding:'10px 16px', background:'#F3F4F6', font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em', color:'#6B7280', borderBottom:'1px solid #E5E7EB'}}>{h}</th>
          ))}</tr></thead>
          <tbody>{rows.map((r,i)=>(
            <tr key={i} onMouseEnter={e=>e.currentTarget.style.background='#F8F9FA'} onMouseLeave={e=>e.currentTarget.style.background='transparent'}>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontWeight:500}}>{r.name}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280'}}>{r.building}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280', fontVariantNumeric:'tabular-nums'}}>{r.floor}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#6B7280'}}>{r.loc}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.cap}</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', color:'#1A1A2E', fontVariantNumeric:'tabular-nums'}}>{r.eq} items</td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2'}}><Badge status={r.s}/></td>
              <td style={{padding:'12px 16px', borderBottom:i===rows.length-1?0:'1px solid #EEF0F2', textAlign:'right'}}><div style={{display:'inline-flex', gap:6, color:'#6B7280'}}><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.edit}</button><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.trash}</button></div></td>
            </tr>
          ))}</tbody>
        </table>
      </Card>
    </>
  );
};

Object.assign(window, { CourseMgmt, EnrollmentMgmt, TrackMgmt, ClassroomMgmt });
