import { useState } from "react";
import { brand, font, formatTime, formatDate, type TripResponse, type User } from "../lib/api";
import TripActionsModal from "./TripActionsModal";

interface Props {
  trips: TripResponse[];
  loading: boolean;
  user: User;
  onAction: () => void;
}

export default function PendingTrips({ trips, loading, user, onAction }: Props) {
  const [managingTrip, setManagingTrip] = useState<TripResponse | null>(null);
  const pending = trips
    .filter(t => t.status?.toLowerCase() === "pending")
    .sort((a, b) => new Date(a.scheduledPickupTime).getTime() - new Date(b.scheduledPickupTime).getTime());

  return (
    <div style={{ background: brand.white, border: `2px solid ${brand.gold}30`, borderRadius: 12, overflow: "hidden", boxShadow: "0 1px 4px rgba(0,0,0,0.05)" }}>
      <div style={{ padding: "18px 24px", borderBottom: `1px solid ${brand.borderGray}`, display: "flex", justifyContent: "space-between", alignItems: "center", background: brand.goldLight }}>
        <div>
          <h3 style={{ margin: 0, fontSize: 16, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>⏳ Pending Trips</h3>
          <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>Admin can cancel these trips</p>
        </div>
        <span style={{ background: brand.gold, color: brand.white, fontWeight: 700, fontSize: 13, padding: "4px 12px", borderRadius: 99, fontFamily: font }}>
          {loading ? "…" : pending.length}
        </span>
      </div>

      {loading ? (
        <div style={{ padding: "36px 24px", textAlign: "center", color: brand.medGray, fontSize: 14, fontFamily: font }}>Loading…</div>
      ) : pending.length === 0 ? (
        <div style={{ padding: "36px 24px", textAlign: "center" }}>
          <div style={{ fontSize: 32, marginBottom: 8 }}>✅</div>
          <div style={{ fontSize: 14, color: brand.medGray, fontFamily: font }}>All caught up — no pending trips!</div>
        </div>
      ) : (
        <div>
          {pending.map((trip, i) => (
            <div key={trip.id} style={{ padding: "14px 24px", borderBottom: i < pending.length - 1 ? `1px solid ${brand.borderGray}` : "none", display: "flex", alignItems: "center", gap: 14 }}>
              <div style={{ width: 40, height: 40, borderRadius: 10, flexShrink: 0, background: brand.goldLight, border: `1.5px solid ${brand.gold}40`, display: "flex", alignItems: "center", justifyContent: "center", fontSize: 18 }}>🚐</div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div style={{ fontSize: 13, fontWeight: 600, color: brand.charcoal, fontFamily: font, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                  {trip.pickupAddress} → {trip.destinationAddress}
                </div>
                <div style={{ fontSize: 12, color: brand.medGray, marginTop: 2, fontFamily: font }}>
                  {formatDate(trip.scheduledPickupTime)} at {formatTime(trip.scheduledPickupTime)}
                  {trip.passengerCount > 0 && ` · ${trip.passengerCount} pax`}
                  {trip.requiresWheelchair && " · ♿"}
                </div>
              </div>
              <div style={{ display: "flex", gap: 8, flexShrink: 0 }}>
                <button onClick={() => setManagingTrip(trip)} style={{ padding: "6px 14px", fontSize: 12, fontWeight: 700, cursor: "pointer", borderRadius: 7, border: "none", background: "#ea580c", color: brand.white, fontFamily: font }}>🚫 Cancel</button>
              </div>
            </div>
          ))}
        </div>
      )}

      {managingTrip && (
        <TripActionsModal trip={managingTrip} user={user} onClose={() => setManagingTrip(null)} onDone={() => { setManagingTrip(null); onAction(); }} />
      )}
    </div>
  );
}
