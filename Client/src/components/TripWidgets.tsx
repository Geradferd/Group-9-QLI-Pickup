import { brand, font, formatTime, formatDate, statusColor, type TripResponse } from "../lib/api";
import { StatusBadge } from "./Cards";

export function TodaysTrips({ trips, loading }: { trips: TripResponse[]; loading: boolean }) {
  const today = new Date().toDateString();
  const todayTrips = trips
    .filter(t => new Date(t.scheduledPickupTime).toDateString() === today)
    .sort((a, b) => new Date(a.scheduledPickupTime).getTime() - new Date(b.scheduledPickupTime).getTime());

  return (
    <div style={{
      background: brand.white,
      border: `1px solid ${brand.borderGray}`,
      borderRadius: 12,
      overflow: "hidden",
      boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
    }}>
      <div style={{
        padding: "18px 24px",
        borderBottom: `1px solid ${brand.borderGray}`,
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}>
        <div>
          <h3 style={{ margin: 0, fontSize: 16, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>Today's Trips</h3>
          <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>
            {new Date().toLocaleDateString([], { weekday: "long", month: "long", day: "numeric" })}
          </p>
        </div>
        <span style={{
          background: brand.blueLight,
          color: brand.blue,
          fontWeight: 700,
          fontSize: 13,
          padding: "4px 12px",
          borderRadius: 99,
          fontFamily: font,
        }}>{todayTrips.length} trips</span>
      </div>

      {loading ? (
        <div style={{ padding: "48px 24px", textAlign: "center", color: brand.medGray, fontSize: 14, fontFamily: font }}>
          Loading trips…
        </div>
      ) : todayTrips.length === 0 ? (
        <div style={{ padding: "48px 24px", textAlign: "center" }}>
          <div style={{ fontSize: 36, marginBottom: 8 }}>🗓️</div>
          <div style={{ fontSize: 15, color: brand.medGray, fontFamily: font }}>No trips scheduled for today</div>
        </div>
      ) : (
        <div style={{ overflowX: "auto" }}>
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 14, fontFamily: font }}>
            <thead>
              <tr style={{ background: brand.lightGray }}>
                {["Time", "Pickup", "Destination", "Pax", "Status"].map(h => (
                  <th key={h} style={{
                    padding: "10px 16px",
                    textAlign: "left",
                    fontSize: 12,
                    fontWeight: 600,
                    color: brand.medGray,
                    textTransform: "uppercase",
                    letterSpacing: "0.5px",
                    whiteSpace: "nowrap",
                  }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {todayTrips.map((trip, i) => (
                <tr key={trip.id} style={{
                  borderTop: `1px solid ${brand.borderGray}`,
                  background: i % 2 === 0 ? brand.white : "#fafafa",
                }}>
                  <td style={{ padding: "12px 16px", whiteSpace: "nowrap", fontWeight: 600, color: brand.charcoal }}>
                    {formatTime(trip.scheduledPickupTime)}
                  </td>
                  <td style={{ padding: "12px 16px", maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", color: brand.darkGray }}>
                    {trip.pickupAddress}
                  </td>
                  <td style={{ padding: "12px 16px", maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", color: brand.darkGray }}>
                    {trip.destinationAddress}
                  </td>
                  <td style={{ padding: "12px 16px", textAlign: "center", color: brand.darkGray }}>
                    {trip.passengerCount}
                    {trip.requiresWheelchair && <span title="Wheelchair required" style={{ marginLeft: 6 }}>♿</span>}
                  </td>
                  <td style={{ padding: "12px 16px" }}>
                    <StatusBadge status={trip.status} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export function RecentTrips({ trips, loading }: { trips: TripResponse[]; loading: boolean }) {
  const recent = [...trips]
    .sort((a, b) => new Date(b.scheduledPickupTime).getTime() - new Date(a.scheduledPickupTime).getTime())
    .slice(0, 6);

  return (
    <div style={{
      background: brand.white,
      border: `1px solid ${brand.borderGray}`,
      borderRadius: 12,
      overflow: "hidden",
      boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
    }}>
      <div style={{ padding: "18px 20px", borderBottom: `1px solid ${brand.borderGray}` }}>
        <h3 style={{ margin: 0, fontSize: 16, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>Recent Activity</h3>
        <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>Latest trip requests</p>
      </div>

      {loading ? (
        <div style={{ padding: "40px 20px", textAlign: "center", color: brand.medGray, fontSize: 14, fontFamily: font }}>Loading…</div>
      ) : recent.length === 0 ? (
        <div style={{ padding: "40px 20px", textAlign: "center" }}>
          <div style={{ fontSize: 28, marginBottom: 8 }}>🚗</div>
          <div style={{ fontSize: 14, color: brand.medGray, fontFamily: font }}>No trips yet</div>
        </div>
      ) : (
        recent.map((trip) => (
          <div key={trip.id} style={{
            padding: "14px 20px",
            borderBottom: `1px solid ${brand.borderGray}`,
            display: "flex",
            alignItems: "center",
            gap: 12,
          }}>
            <div style={{
              width: 36,
              height: 36,
              borderRadius: 8,
              background: statusColor(trip.status).bg,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              flexShrink: 0,
              fontSize: 16,
            }}>🚐</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 13, fontWeight: 500, color: brand.charcoal, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", fontFamily: font }}>
                {trip.pickupAddress} → {trip.destinationAddress}
              </div>
              <div style={{ fontSize: 12, color: brand.medGray, marginTop: 2, fontFamily: font }}>
                {formatDate(trip.scheduledPickupTime)} at {formatTime(trip.scheduledPickupTime)}
              </div>
            </div>
            <StatusBadge status={trip.status} />
          </div>
        ))
      )}
    </div>
  );
}

export function TripStatusSummary({ trips, loading }: { trips: TripResponse[]; loading: boolean }) {
  return (
    <div style={{
      background: brand.white,
      border: `1px solid ${brand.borderGray}`,
      borderRadius: 12,
      padding: "20px",
      boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
    }}>
      <h3 style={{ margin: "0 0 16px", fontSize: 15, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>
        Trip Status Summary
      </h3>
      {loading ? (
        <div style={{ color: brand.medGray, fontSize: 13, fontFamily: font }}>Loading…</div>
      ) : (() => {
        const groups: Record<string, number> = {};
        trips.forEach(t => { const s = t.status || "Unknown"; groups[s] = (groups[s] || 0) + 1; });
        const total = trips.length;
        return (
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {Object.entries(groups).map(([status, count]) => {
              const pct = total > 0 ? Math.round((count / total) * 100) : 0;
              const c = statusColor(status);
              return (
                <div key={status}>
                  <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 4, fontSize: 13, fontFamily: font }}>
                    <span style={{ color: brand.darkGray, fontWeight: 500 }}>{status}</span>
                    <span style={{ color: brand.medGray }}>{count} ({pct}%)</span>
                  </div>
                  <div style={{ height: 6, background: brand.lightGray, borderRadius: 99, overflow: "hidden" }}>
                    <div style={{ height: "100%", width: `${pct}%`, background: c.dot, borderRadius: 99, transition: "width 0.6s ease" }} />
                  </div>
                </div>
              );
            })}
            {total === 0 && <div style={{ color: brand.medGray, fontSize: 13, fontFamily: font }}>No trips found</div>}
          </div>
        );
      })()}
    </div>
  );
}
