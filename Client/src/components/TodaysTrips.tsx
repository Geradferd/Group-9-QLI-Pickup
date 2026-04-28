import { useState } from "react";
import { brand, font, formatTime, type TripResponse, type User } from "../lib/api";
import { StatusBadge } from "./Cards";
import TripActionsModal from "./TripActionsModal";

interface Props {
  trips: TripResponse[];
  loading: boolean;
  user?: User;
  onAction?: () => void;
}

export default function TodaysTrips({ trips, loading, user, onAction }: Props) {
  const [managingTrip, setManagingTrip] = useState<TripResponse | null>(null);
  const today = new Date().toDateString();
  const isAdmin = user?.role?.toLowerCase() === "admin";
  const todayTrips = trips
    .filter(t => new Date(t.scheduledPickupTime).toDateString() === today)
    .sort((a, b) => new Date(a.scheduledPickupTime).getTime() - new Date(b.scheduledPickupTime).getTime());

  return (
    <div style={{ background: brand.white, border: `1px solid ${brand.borderGray}`, borderRadius: 12, overflow: "hidden", boxShadow: "0 1px 4px rgba(0,0,0,0.05)" }}>
      <div style={{ padding: "18px 24px", borderBottom: `1px solid ${brand.borderGray}`, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <div>
          <h3 style={{ margin: 0, fontSize: 16, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>Today's Trips</h3>
          <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>
            {new Date().toLocaleDateString([], { weekday: "long", month: "long", day: "numeric" })}
          </p>
        </div>
        <span style={{ background: brand.blueLight, color: brand.blue, fontWeight: 700, fontSize: 13, padding: "4px 12px", borderRadius: 99, fontFamily: font }}>
          {todayTrips.length} trips
        </span>
      </div>

      {loading ? (
        <div style={{ padding: "48px 24px", textAlign: "center", color: brand.medGray, fontSize: 14, fontFamily: font }}>Loading trips…</div>
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
                {["Time", "Pickup", "Destination", "Pax", "Status", ...(isAdmin ? ["Action"] : [])].map(h => (
                  <th key={h} style={{ padding: "10px 16px", textAlign: "left", fontSize: 12, fontWeight: 600, color: brand.medGray, textTransform: "uppercase", letterSpacing: "0.5px", whiteSpace: "nowrap" }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {todayTrips.map((trip, i) => (
                <tr key={trip.id} style={{ borderTop: `1px solid ${brand.borderGray}`, background: i % 2 === 0 ? brand.white : "#fafafa" }}>
                  <td style={{ padding: "12px 16px", whiteSpace: "nowrap", fontWeight: 600, color: brand.charcoal }}>{formatTime(trip.scheduledPickupTime)}</td>
                  <td style={{ padding: "12px 16px", maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", color: brand.darkGray }}>{trip.pickupAddress}</td>
                  <td style={{ padding: "12px 16px", maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", color: brand.darkGray }}>{trip.destinationAddress}</td>
                  <td style={{ padding: "12px 16px", textAlign: "center", color: brand.darkGray }}>
                    {trip.passengerCount}
                    {trip.requiresWheelchair && <span title="Wheelchair required" style={{ marginLeft: 6 }}>♿</span>}
                  </td>
                  <td style={{ padding: "12px 16px" }}><StatusBadge status={trip.status} /></td>
                  {isAdmin && (
                    <td style={{ padding: "12px 16px" }}>
                      <button onClick={() => setManagingTrip(trip)} style={{ padding: "5px 12px", fontSize: 12, fontWeight: 600, border: `1px solid ${brand.blue}`, borderRadius: 6, background: brand.blueLight, color: brand.blue, cursor: "pointer", fontFamily: font, whiteSpace: "nowrap" }}>
                        Manage →
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {managingTrip && user && (
        <TripActionsModal trip={managingTrip} user={user} onClose={() => setManagingTrip(null)} onDone={() => { setManagingTrip(null); onAction?.(); }} />
      )}
    </div>
  );
}
