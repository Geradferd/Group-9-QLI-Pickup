import { useState, useEffect } from "react";
import { apiCall, brand, font, type User, type TripResponse, type NotificationResponse } from "../lib/api";
import { StatCard, QuickAction } from "../components/Cards";
import { TodaysTrips, RecentTrips, TripStatusSummary } from "../components/TripWidgets";
import { NotificationsPanel } from "../components/NotificationsPanel";
import NewTripModal from "../components/NewTripModal";

// ── Types ─────────────────────────────────────────────────────────────────────
export type Page = "dashboard" | "riders" | "drivers" | "vehicles" | "trips" | "schedule" | "reports";

// ── Icons ─────────────────────────────────────────────────────────────────────
const IconTrip = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="10" /><polyline points="12 6 12 12 16 14" />
  </svg>
);
const IconPending = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
    <line x1="12" y1="9" x2="12" y2="13" /><line x1="12" y1="17" x2="12.01" y2="17" />
  </svg>
);
const IconComplete = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="20 6 9 17 4 12" />
  </svg>
);
const IconRider = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
    <path d="M23 21v-2a4 4 0 0 0-3-3.87" /><path d="M16 3.13a4 4 0 0 1 0 7.75" />
  </svg>
);
const IconCar = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="1" y="3" width="15" height="13" rx="2" />
    <polygon points="16 8 20 8 23 11 23 16 16 16 16 8" />
    <circle cx="5.5" cy="18.5" r="2.5" /><circle cx="18.5" cy="18.5" r="2.5" />
  </svg>
);
const IconBell = () => (
  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" /><path d="M13.73 21a2 2 0 0 1-3.46 0" />
  </svg>
);
const IconRefresh = ({ spinning }: { spinning: boolean }) => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"
    style={spinning ? { animation: "spin 1s linear infinite" } : {}}>
    <polyline points="23 4 23 10 17 10" /><polyline points="1 20 1 14 7 14" />
    <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
  </svg>
);

// ── Dashboard Page ────────────────────────────────────────────────────────────
export default function Dashboard({
  user,
  onLogout,
  onNavigate,
}: {
  user: User;
  onLogout: () => void;
  onNavigate: (page: Page) => void;
}) {
  const [trips, setTrips] = useState<TripResponse[]>([]);
  const [notifications, setNotifications] = useState<NotificationResponse[]>([]);
  const [tripsLoading, setTripsLoading] = useState(true);
  const [notifsLoading, setNotifsLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [showNewTrip, setShowNewTrip] = useState(false);

  const isAdmin = user.role?.toLowerCase() === "admin";
  const isDriver = user.role?.toLowerCase() === "driver";

  const loadData = async (showRefresh = false) => {
    if (showRefresh) setRefreshing(true);
    try {
      const [tripsData, notifsData] = await Promise.all([
        apiCall("/trips", "GET", null, user.token),
        apiCall("/notifications?pageSize=20", "GET", null, user.token),
      ]);
      setTrips(tripsData ?? []);
      setNotifications(notifsData?.items ?? notifsData ?? []);
    } catch { /* network may not be up yet */ }
    setTripsLoading(false);
    setNotifsLoading(false);
    if (showRefresh) setRefreshing(false);
  };

  useEffect(() => {
    loadData();
    const interval = setInterval(() => loadData(), 60_000);
    return () => clearInterval(interval);
  }, []);

  const markRead = async (id: number) => {
    try {
      await apiCall(`/notifications/${id}/read`, "POST", null, user.token);
      setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n));
    } catch { /* ignore */ }
  };

  // Computed stats
  const today        = new Date().toDateString();
  const todayTrips   = trips.filter(t => new Date(t.scheduledPickupTime).toDateString() === today);
  const pendingTrips = trips.filter(t => t.status?.toLowerCase() === "pending");
  const completedToday = todayTrips.filter(t => t.status?.toLowerCase() === "completed");
  const activeTrips  = trips.filter(t => t.status?.toLowerCase() === "inprogress");
  const unreadCount  = notifications.filter(n => !n.isRead).length;

  const greeting = new Date().getHours() < 12 ? "morning" : new Date().getHours() < 17 ? "afternoon" : "evening";

  const navItems: { label: string; page: Page }[] = [
    { label: "Dashboard", page: "dashboard" },
    { label: "Trips",     page: "trips" },
    { label: "Schedule",  page: "schedule" },
    ...(isAdmin || isDriver ? [{ label: "Riders",   page: "riders"   as Page }] : []),
    ...(isAdmin             ? [{ label: "Drivers",  page: "drivers"  as Page }] : []),
    ...(isAdmin             ? [{ label: "Vehicles", page: "vehicles" as Page }] : []),
    ...(isAdmin             ? [{ label: "Reports",  page: "reports"  as Page }] : []),
  ];

  return (
    <div style={{ minHeight: "100vh", background: brand.lightGray, fontFamily: font }}>
      <style>{`
        @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
        * { box-sizing: border-box; }
        ::-webkit-scrollbar { width: 6px; height: 6px; }
        ::-webkit-scrollbar-thumb { background: #ccc; border-radius: 3px; }
      `}</style>

      {/* Top accent bar */}
      <div style={{ height: 4, background: `linear-gradient(90deg, ${brand.blue} 0%, ${brand.green} 45%, ${brand.gold} 100%)` }} />

      {/* Header */}
      <header style={{
        background: brand.white, borderBottom: `1px solid ${brand.borderGray}`,
        padding: "0 32px", display: "flex", justifyContent: "space-between",
        alignItems: "center", height: 64, position: "sticky", top: 0, zIndex: 100,
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
          <img src="/qli-checkmark.png" alt="QLI" style={{ width: 34, height: 34 }} />
          <span style={{ fontSize: 20, fontWeight: 700, color: brand.charcoal, letterSpacing: "-0.3px", fontFamily: font }}>
            QLIFT
          </span>
        </div>

        <nav style={{ display: "flex", gap: 4 }}>
          {navItems.map(item => (
            <button key={item.page} onClick={() => onNavigate(item.page)} style={{
              padding: "6px 14px", fontSize: 14,
              fontWeight: item.page === "dashboard" ? 600 : 400,
              color: item.page === "dashboard" ? brand.blue : brand.darkGray,
              background: item.page === "dashboard" ? brand.blueLight : "transparent",
              border: "none", borderRadius: 6, cursor: "pointer", fontFamily: font,
            }}>{item.label}</button>
          ))}
        </nav>

        <div style={{ display: "flex", alignItems: "center", gap: 14 }}>
          {/* Bell */}
          <div style={{ position: "relative" }}>
            <button style={{
              width: 38, height: 38, borderRadius: 8, border: `1px solid ${brand.borderGray}`,
              background: brand.white, cursor: "pointer",
              display: "flex", alignItems: "center", justifyContent: "center", color: brand.medGray,
            }}><IconBell /></button>
            {unreadCount > 0 && (
              <span style={{
                position: "absolute", top: -4, right: -4,
                background: brand.red, color: brand.white, fontSize: 10, fontWeight: 700,
                width: 18, height: 18, borderRadius: "50%",
                display: "flex", alignItems: "center", justifyContent: "center",
                border: `2px solid ${brand.white}`,
              }}>{unreadCount > 9 ? "9+" : unreadCount}</span>
            )}
          </div>
          <div style={{ width: 1, height: 28, background: brand.borderGray }} />
          {/* Avatar */}
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <div style={{
              width: 36, height: 36, borderRadius: "50%",
              background: `linear-gradient(135deg, ${brand.blue}, ${brand.green})`,
              display: "flex", alignItems: "center", justifyContent: "center",
              color: brand.white, fontWeight: 700, fontSize: 14,
            }}>{user.displayName?.charAt(0).toUpperCase()}</div>
            <div>
              <div style={{ fontSize: 14, fontWeight: 500, color: brand.charcoal, lineHeight: 1.2, fontFamily: font }}>{user.displayName}</div>
              <div style={{ fontSize: 11, color: brand.blue, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.4px", fontFamily: font }}>{user.role}</div>
            </div>
          </div>
          <button onClick={onLogout} style={{
            padding: "7px 16px", fontSize: 13, fontWeight: 500, color: brand.medGray,
            background: "transparent", border: `1px solid ${brand.borderGray}`,
            borderRadius: 6, cursor: "pointer", fontFamily: font,
          }}>Sign out</button>
        </div>
      </header>

      {/* Page body */}
      <main style={{ maxWidth: 1280, margin: "0 auto", padding: "32px 32px 48px" }}>

        {/* Heading row */}
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 28 }}>
          <div>
            <h1 style={{ margin: 0, fontSize: 24, fontWeight: 700, color: brand.charcoal, fontFamily: font }}>
              Good {greeting}, {user.displayName.split(" ")[0]} 👋
            </h1>
            <p style={{ margin: "4px 0 0", fontSize: 14, color: brand.medGray, fontFamily: font }}>
              Here's what's happening with QLIFT today.
            </p>
          </div>
          <button onClick={() => loadData(true)} style={{
            display: "flex", alignItems: "center", gap: 6,
            padding: "8px 16px", fontSize: 13, fontWeight: 500, color: brand.medGray,
            background: brand.white, border: `1px solid ${brand.borderGray}`,
            borderRadius: 8, cursor: "pointer", fontFamily: font,
          }}>
            <IconRefresh spinning={refreshing} /> Refresh
          </button>
        </div>

        {/* Stat cards */}
        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 16, marginBottom: 28 }}>
          <StatCard label="Today's Trips"    value={tripsLoading ? "—" : todayTrips.length}     icon={<IconTrip />}     color={brand.blue}   bg={brand.blueLight}   sub={`${completedToday.length} completed`} />
          <StatCard label="Pending Approval" value={tripsLoading ? "—" : pendingTrips.length}   icon={<IconPending />}  color={brand.gold}   bg={brand.goldLight}   sub="Needs attention" />
          <StatCard label="Active Trips"     value={tripsLoading ? "—" : activeTrips.length}    icon={<IconCar />}      color={brand.purple} bg={brand.purpleLight} sub="In progress now" />
          <StatCard label="Completed Today"  value={tripsLoading ? "—" : completedToday.length} icon={<IconComplete />} color={brand.green}  bg={brand.greenLight}  sub={`of ${todayTrips.length} scheduled`} />
        </div>

        {/* Quick actions */}
        <div style={{
          background: brand.white, border: `1px solid ${brand.borderGray}`,
          borderRadius: 12, padding: "20px 24px", marginBottom: 28,
          boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
        }}>
          <h3 style={{ margin: "0 0 16px", fontSize: 15, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>
            Quick Actions
          </h3>
          <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(120px, 1fr))", gap: 12 }}>
            <QuickAction label="New Trip"  icon={<span style={{ fontSize: 20 }}>➕</span>} color={brand.blue}   bg={brand.blueLight}   onClick={() => setShowNewTrip(true)} />
            <QuickAction label="Schedule"  icon={<span style={{ fontSize: 20 }}>📅</span>} color={brand.green}  bg={brand.greenLight}  onClick={() => onNavigate("schedule")} />
            {(isAdmin || isDriver) && (
              <QuickAction label="Riders" icon={<IconRider />} color={brand.gold} bg={brand.goldLight} onClick={() => onNavigate("riders")} />
            )}
            {isAdmin && <>
              <QuickAction label="Drivers"  icon={<span style={{ fontSize: 20 }}>🧑‍✈️</span>} color={brand.purple} bg={brand.purpleLight} onClick={() => onNavigate("drivers")} />
              <QuickAction label="Vehicles" icon={<IconCar />}                                color={brand.blue}   bg={brand.blueLight}   onClick={() => onNavigate("vehicles")} />
              <QuickAction label="Reports"  icon={<span style={{ fontSize: 20 }}>📊</span>}  color={brand.green}  bg={brand.greenLight}  onClick={() => onNavigate("reports")} />
            </>}
          </div>
        </div>

        {/* Main 2-column grid */}
        <div style={{ display: "grid", gridTemplateColumns: "1fr 340px", gap: 20, alignItems: "start" }}>
          <div style={{ display: "flex", flexDirection: "column", gap: 20 }}>
            <TodaysTrips trips={trips} loading={tripsLoading} />
            <RecentTrips trips={trips} loading={tripsLoading} />
          </div>
          <div style={{ display: "flex", flexDirection: "column", gap: 20 }}>
            <NotificationsPanel notifications={notifications} loading={notifsLoading} onMarkRead={markRead} />
            <TripStatusSummary trips={trips} loading={tripsLoading} />
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer style={{
        padding: "16px 32px", borderTop: `1px solid ${brand.borderGray}`,
        background: brand.white, display: "flex", justifyContent: "space-between",
        fontSize: 12, color: brand.medGray, fontFamily: font,
      }}>
        <span>© 2026 Quality Living, Inc.</span>
        <span>QLIFT v1.0</span>
      </footer>

      {/* New Trip Modal */}
      {showNewTrip && (
        <NewTripModal
          user={user}
          onClose={() => setShowNewTrip(false)}
          onCreated={() => loadData()}
        />
      )}
    </div>
  );
}
