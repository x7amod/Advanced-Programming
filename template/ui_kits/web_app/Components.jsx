/* global React */
const { useState } = React;

// ============ Icons (Lucide-style inline SVG) ============
const Icon = ({ d, size = 18, stroke = 1.6, ...rest }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor"
       strokeWidth={stroke} strokeLinecap="round" strokeLinejoin="round" {...rest}>
    {typeof d === 'string' ? <path d={d}/> : d}
  </svg>
);
const I = {
  home:      <Icon d={<><path d="M3 12 12 4l9 8"/><path d="M5 10v9h14v-9"/></>}/>,
  book:      <Icon d={<><path d="M4 5h12a3 3 0 0 1 3 3v11H7a3 3 0 0 1-3-3z"/><path d="M4 5v11"/></>}/>,
  cal:       <Icon d={<><rect x="4" y="5" width="16" height="15" rx="2"/><path d="M8 3v4M16 3v4M4 10h16"/></>}/>,
  award:     <Icon d={<><circle cx="12" cy="9" r="5"/><path d="M9 14l-2 7 5-3 5 3-2-7"/></>}/>,
  card:      <Icon d={<><rect x="3" y="6" width="18" height="13" rx="2"/><path d="M3 10h18"/></>}/>,
  users:     <Icon d={<><circle cx="9" cy="9" r="3.5"/><path d="M2.5 19a6.5 6.5 0 0 1 13 0"/><circle cx="17" cy="10" r="2.8"/><path d="M21.5 18a4.5 4.5 0 0 0-6-3.4"/></>}/>,
  clip:      <Icon d={<><rect x="6" y="4" width="12" height="17" rx="2"/><path d="M9 4h6v3H9z"/><path d="M9 12l2 2 4-4"/></>}/>,
  bar:       <Icon d={<><path d="M4 20V10M10 20V4M16 20v-7M22 20H2"/></>}/>,
  bell:      <Icon d={<><path d="M6 16h12l-1.4-2V11a4.6 4.6 0 0 0-9.2 0v3z"/><path d="M10 19a2 2 0 0 0 4 0"/></>}/>,
  search:    <Icon d={<><circle cx="11" cy="11" r="6"/><path d="m20 20-3.5-3.5"/></>}/>,
  chev:      <Icon d="m9 6 6 6-6 6"/>,
  chevDown:  <Icon d="m6 9 6 6 6-6"/>,
  x:         <Icon d={<><path d="M6 6l12 12M18 6 6 18"/></>}/>,
  check:     <Icon d="M5 12l5 5L20 7"/>,
  plus:      <Icon d={<><path d="M12 5v14M5 12h14"/></>}/>,
  edit:      <Icon d={<><path d="M4 20h4l10-10-4-4L4 16z"/></>}/>,
  trash:     <Icon d={<><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13"/></>}/>,
  download:  <Icon d={<><path d="M12 4v12"/><path d="m7 11 5 5 5-5"/><path d="M5 20h14"/></>}/>,
  logout:    <Icon d={<><path d="M14 4h5v16h-5"/><path d="m10 8-4 4 4 4"/><path d="M6 12h11"/></>}/>,
  user:      <Icon d={<><circle cx="12" cy="8" r="4"/><path d="M4 21a8 8 0 0 1 16 0"/></>}/>,
  cog:       <Icon d={<><circle cx="12" cy="12" r="3"/><path d="M19 12a7 7 0 0 0-.1-1.3l2-1.5-2-3.5-2.4.8a7 7 0 0 0-2.2-1.3L13.8 3h-3.6l-.5 2.2a7 7 0 0 0-2.2 1.3l-2.4-.8-2 3.5 2 1.5A7 7 0 0 0 5 12c0 .5 0 .9.1 1.3l-2 1.5 2 3.5 2.4-.8a7 7 0 0 0 2.2 1.3l.5 2.2h3.6l.5-2.2a7 7 0 0 0 2.2-1.3l2.4.8 2-3.5-2-1.5c.1-.4.1-.8.1-1.3z"/></>}/>,
  mark:      <Icon d={<><path d="M12 2 5 5v6c0 4.5 3 8.5 7 11 4-2.5 7-6.5 7-11V5z"/></>}/>,
  clock:     <Icon d={<><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/></>}/>,
};

// ============ Buttons ============
const Btn = ({ variant='primary', size='md', icon, children, onClick, type='button', disabled, full }) => {
  const base = {
    display:'inline-flex', alignItems:'center', gap:8, justifyContent:'center',
    border:'1px solid transparent', cursor: disabled?'not-allowed':'pointer',
    borderRadius:8, transition:'background 150ms cubic-bezier(.4,0,.2,1), border-color 150ms',
    font:'500 14px/20px Inter, sans-serif',
    padding: size==='sm' ? '6px 12px' : '9px 16px',
    width: full ? '100%' : 'auto',
    opacity: disabled ? 0.5 : 1,
  };
  const variants = {
    primary:   { background:'#0D9488', color:'#fff' },
    secondary: { background:'#fff', color:'#0D9488', borderColor:'#0D9488' },
    ghost:     { background:'transparent', color:'#1A1A2E' },
    danger:    { background:'#EF4444', color:'#fff' },
    soft:      { background:'#F3F4F6', color:'#1A1A2E' },
  };
  return (
    <button type={type} disabled={disabled} onClick={onClick}
      style={{...base, ...variants[variant]}}
      onMouseEnter={e => {
        if(disabled) return;
        if(variant==='primary') e.currentTarget.style.background='#0F766E';
        if(variant==='secondary') e.currentTarget.style.background='rgba(13,148,136,0.08)';
        if(variant==='ghost') e.currentTarget.style.background='#F3F4F6';
        if(variant==='danger') e.currentTarget.style.background='#DC2626';
        if(variant==='soft') e.currentTarget.style.background='#E5E7EB';
      }}
      onMouseLeave={e => {
        e.currentTarget.style.background = variants[variant].background;
      }}
    >
      {icon && <span style={{display:'inline-flex'}}>{icon}</span>}
      {children}
    </button>
  );
};

// ============ Status badge ============
const STATUS_MAP = {
  Enrolled:    { bg:'#D1FAE5', fg:'#065F46', dot:'#10B981' },
  Waitlisted:  { bg:'#FEF3C7', fg:'#92400E', dot:'#F59E0B' },
  Completed:   { bg:'#DBEAFE', fg:'#1E40AF', dot:'#3B82F6' },
  Dropped:     { bg:'#F3F4F6', fg:'#374151', dot:'#6B7280' },
  Cancelled:   { bg:'#FEE2E2', fg:'#991B1B', dot:'#EF4444' },
  Pending:     { bg:'#FEF3C7', fg:'#92400E', dot:'#F59E0B' },
  Paid:        { bg:'#D1FAE5', fg:'#065F46', dot:'#10B981' },
  'Partially paid': { bg:'#DBEAFE', fg:'#1E40AF', dot:'#3B82F6' },
  Overdue:     { bg:'#FEE2E2', fg:'#991B1B', dot:'#EF4444' },
  Refunded:    { bg:'#F3F4F6', fg:'#374151', dot:'#6B7280' },
  Scheduled:   { bg:'#DBEAFE', fg:'#1E40AF', dot:'#3B82F6' },
  'In progress': { bg:'#CCFBF1', fg:'#0F766E', dot:'#0D9488' },
  Awarded:     { bg:'#D1FAE5', fg:'#065F46', dot:'#10B981' },
  Expired:     { bg:'#FEE2E2', fg:'#991B1B', dot:'#EF4444' },
  Pass:        { bg:'#D1FAE5', fg:'#065F46', dot:'#10B981' },
  Fail:        { bg:'#FEE2E2', fg:'#991B1B', dot:'#EF4444' },
  Active:      { bg:'#D1FAE5', fg:'#065F46', dot:'#10B981' },
  Inactive:    { bg:'#F3F4F6', fg:'#374151', dot:'#6B7280' },
  Trainee:     { bg:'#DBEAFE', fg:'#1E40AF' },
  Instructor:  { bg:'#CCFBF1', fg:'#0F766E' },
  Coordinator: { bg:'#FEF3C7', fg:'#92400E' },
};
const Badge = ({ status, dot=true }) => {
  const c = STATUS_MAP[status] || { bg:'#F3F4F6', fg:'#374151' };
  return (
    <span style={{
      display:'inline-flex', alignItems:'center', gap:6, padding:'2px 10px',
      borderRadius:9999, font:'600 12px/16px Inter, sans-serif',
      background:c.bg, color:c.fg
    }}>
      {dot && c.dot && <span style={{width:6, height:6, borderRadius:999, background:c.dot}}/>}
      {status}
    </span>
  );
};

// ============ Card ============
const Card = ({ title, action, children, padded=true, style={} }) => (
  <div style={{
    background:'#fff', border:'1px solid #E5E7EB', borderRadius:12,
    boxShadow:'0 1px 3px rgba(0,0,0,0.06), 0 1px 2px rgba(0,0,0,0.04)',
    padding: padded ? 20 : 0, ...style
  }}>
    {title && (
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center',
                   marginBottom:14, padding: padded ? 0 : '16px 20px 0'}}>
        <div style={{font:'600 16px/24px Inter, sans-serif', color:'#1A1A2E'}}>{title}</div>
        {action}
      </div>
    )}
    {children}
  </div>
);

// ============ Field / Input ============
const Field = ({ label, hint, error, children }) => (
  <div>
    {label && <label style={{display:'block', font:'600 12px/16px Inter, sans-serif', color:'#1A1A2E', marginBottom:6}}>{label}</label>}
    {children}
    {hint && !error && <div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280', marginTop:4}}>{hint}</div>}
    {error && <div style={{font:'500 12px/16px Inter, sans-serif', color:'#991B1B', marginTop:4}}>{error}</div>}
  </div>
);
const Input = (props) => (
  <input {...props} style={{
    width:'100%', padding:'9px 12px', border:'1px solid #E5E7EB', borderRadius:4,
    background:'#fff', font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E',
    outline:'none', ...(props.style||{})
  }}
  onFocus={e => { e.target.style.borderColor='#0D9488'; e.target.style.boxShadow='0 0 0 3px rgba(13,148,136,0.18)'; props.onFocus?.(e); }}
  onBlur={e => { e.target.style.borderColor='#E5E7EB'; e.target.style.boxShadow='none'; props.onBlur?.(e); }}
  />
);
const Select = ({ children, ...props }) => (
  <select {...props} style={{
    width:'100%', padding:'9px 12px', border:'1px solid #E5E7EB', borderRadius:4,
    background:'#fff', font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E', outline:'none',
    appearance:'none', backgroundImage:'url("data:image/svg+xml;utf8,<svg xmlns=%22http://www.w3.org/2000/svg%22 width=%2216%22 height=%2216%22 viewBox=%220 0 24 24%22 fill=%22none%22 stroke=%22%236B7280%22 stroke-width=%221.6%22 stroke-linecap=%22round%22 stroke-linejoin=%22round%22><path d=%22m6 9 6 6 6-6%22/></svg>")', backgroundRepeat:'no-repeat', backgroundPosition:'right 10px center', paddingRight:36
  }}>{children}</select>
);

// ============ Table ============
const Table = ({ columns, rows, onRowClick }) => (
  <div style={{overflow:'hidden', borderRadius:12}}>
    <table style={{width:'100%', borderCollapse:'collapse', font:'400 14px/22px Inter, sans-serif'}}>
      <thead>
        <tr>
          {columns.map((c,i) => (
            <th key={i} style={{
              textAlign: c.align==='right'?'right':'left',
              padding:'10px 16px', background:'#F3F4F6',
              font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.04em',
              color:'#6B7280', borderBottom:'1px solid #E5E7EB',
            }}>{c.header}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows.map((r,ri) => (
          <tr key={ri} onClick={() => onRowClick?.(r)}
              style={{cursor: onRowClick?'pointer':'default'}}
              onMouseEnter={e => e.currentTarget.style.background='#F8F9FA'}
              onMouseLeave={e => e.currentTarget.style.background='transparent'}>
            {columns.map((c,ci) => (
              <td key={ci} style={{
                padding:'12px 16px', borderBottom: ri===rows.length-1?'0':'1px solid #EEF0F2',
                color:'#1A1A2E', textAlign: c.align==='right'?'right':'left',
                fontVariantNumeric: c.num?'tabular-nums':'normal'
              }}>{c.render ? c.render(r) : r[c.key]}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  </div>
);

// ============ Stat tile ============
const Stat = ({ label, value, hint, trend, icon }) => (
  <Card>
    <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-start', marginBottom:10}}>
      <div style={{font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', color:'#6B7280'}}>{label}</div>
      {icon && <div style={{color:'#0D9488'}}>{icon}</div>}
    </div>
    <div style={{font:'700 28px/34px Inter, sans-serif', color:'#1A1A2E', fontVariantNumeric:'tabular-nums', letterSpacing:'-0.01em'}}>{value}</div>
    {hint && <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:4}}>
      {trend && <span style={{color: trend==='up'?'#065F46':'#991B1B', marginRight:6}}>{trend==='up'?'▲':'▼'}</span>}
      {hint}
    </div>}
  </Card>
);

// ============ Avatar ============
const Avatar = ({ name, size=32, color }) => {
  const initials = name.split(' ').map(s=>s[0]).slice(0,2).join('').toUpperCase();
  // Hash to pick a colour from a calm palette
  const palette = ['#0D9488','#0F766E','#3B82F6','#6366F1','#0891B2','#65A30D','#CA8A04'];
  const hash = [...name].reduce((a,c)=>a+c.charCodeAt(0),0);
  return (
    <div style={{
      width:size, height:size, borderRadius:9999,
      background: color || palette[hash % palette.length],
      color:'#fff', display:'inline-flex', alignItems:'center', justifyContent:'center',
      font:`600 ${Math.round(size*0.38)}px/1 Inter, sans-serif`, flex:'none'
    }}>{initials}</div>
  );
};

Object.assign(window, { I, Icon, Btn, Badge, Card, Field, Input, Select, Table, Stat, Avatar });
