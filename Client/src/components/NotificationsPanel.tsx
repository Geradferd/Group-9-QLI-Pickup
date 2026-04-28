import { brand, font, timeAgo, type NotificationResponse } from "../lib/api";

export function NotificationsPanel({
  notifications,
  loading,
  onMarkRead,
}: {
  notifications: NotificationResponse[];
  loading: boolean;
  onMarkRead: (id: number) => void;
}) {
  const unread = notifications.filter(n => !n.isRead);

  return (
    <div style={{
      background: brand.white,
      border: `1px solid ${brand.borderGray}`,
      borderRadius: 12,
      overflow: "hidden",
      boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
      display: "flex",
      flexDirection: "column",
      maxHeight: 420,
    }}>
      <div style={{
        padding: "18px 20px",
        borderBottom: `1px solid ${brand.borderGray}`,
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        flexShrink: 0,
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <h3 style={{ margin: 0, fontSize: 16, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>
            Notifications
          </h3>
          {unread.length > 0 && (
            <span style={{
              background: brand.red,
              color: brand.white,
              fontWeight: 700,
              fontSize: 11,
              padding: "2px 7px",
              borderRadius: 99,
            }}>{unread.length}</span>
          )}
        </div>
      </div>

      <div style={{ overflowY: "auto", flex: 1 }}>
        {loading ? (
          <div style={{ padding: "40px 20px", textAlign: "center", color: brand.medGray, fontSize: 14, fontFamily: font }}>Loading…</div>
        ) : notifications.length === 0 ? (
          <div style={{ padding: "40px 20px", textAlign: "center" }}>
            <div style={{ fontSize: 28, marginBottom: 8 }}>🔔</div>
            <div style={{ fontSize: 14, color: brand.medGray, fontFamily: font }}>No notifications</div>
          </div>
        ) : (
          notifications.map(n => (
            <div
              key={n.id}
              onClick={() => !n.isRead && onMarkRead(n.id)}
              style={{
                padding: "14px 20px",
                borderBottom: `1px solid ${brand.borderGray}`,
                background: n.isRead ? brand.white : "#f0f7ff",
                cursor: n.isRead ? "default" : "pointer",
                transition: "background 0.15s",
                display: "flex",
                gap: 12,
                alignItems: "flex-start",
              }}
            >
              <div style={{
                width: 8,
                height: 8,
                borderRadius: "50%",
                background: n.isRead ? "transparent" : brand.blue,
                marginTop: 5,
                flexShrink: 0,
              }} />
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontSize: 13, fontWeight: n.isRead ? 400 : 600, color: brand.charcoal, marginBottom: 2, fontFamily: font }}>
                  {n.title}
                </div>
                <div style={{ fontSize: 12, color: brand.medGray, lineHeight: 1.4, fontFamily: font }}>{n.body}</div>
                <div style={{ fontSize: 11, color: brand.medGray, marginTop: 4, fontFamily: font }}>{timeAgo(n.createdAt)}</div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
