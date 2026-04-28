import { useState } from "react";
import { brand, font, formatTime, formatDate, statusColor, type TripResponse, type User } from "../lib/api";
import { StatusBadge } from "./Cards";
import TripActionsModal from "./TripActionsModal";

interface Props {
  trips: TripResponse[];
  loading: boolean;
  user?: User;
  onAction?: () => void;
}

export default function RecentTrips({ trips, loading, user, onAction }: Props) {
  const [managingTrip, setManagingTrip] = useState<TripResponse | null>(null);
  const isAdmin = user?.role?.toLowerCase() === "admin";
  const recent = [...trips]
    .sort((a, b) => new Date(b.scheduledPickupTime).getTime() - new Date(a.scheduledPickupTime).getTime())
    .slice(0, 6);

  return (
    <div style={{ background: brand.white, border: `1px solid ${brand.borderGray}`, borderRadius: 12, overflow: "hidden", boxShadow: "0 1px 4px rgba(0,0,0,0.05)" }}>
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
          <div key={trip.id} style={{ padding: "14px 20px", borderBottom: `1px solid ${brand.borderGray}`, display: "flex", alignItems: "center", gap: 12 }}>
            <div style={{ width: 36, height: 36, borderRadius: 8, background: statusColor(trip.status).bg, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0, fontSize: 16 }}>🚐</div>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 13, fontWeight: 500, color: brand.charcoal, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap", fontFamily: font }}>
                {trip.pickupAddress} → {trip.destinationAddress}
              </div>
              <div style={{ fontSize: 12, color: brand.medGray, marginTop: 2, fontFamily: font }}>
                {formatDate(trip.scheduledPickupTime)} at {formatTime(trip.scheduledPickupTime)}
              </div>
            </div>
            <StatusBadge status={trip.status} />
            {isAdmin && (
              <button onClick={() => setManagingTrip(trip)} style={{ padding: "5px 10px", fontSize: 12, fontWeight: 600, border: `1px solid ${brand.blue}`, borderRadius: 6, background: brand.blueLight, color: brand.blue, cursor: "pointer", fontFamily: font, whiteSpace: "nowrap", flexShrink: 0 }}>
                Manage
              </button>
            )}
          </div>
        ))
      )}

      {managingTrip && user && (
        <TripActionsModal trip={managingTrip} user={user} onClose={() => setManagingTrip(null)} onDone={() => { setManagingTrip(null); onAction?.(); }} />
      )}
    </div>
  );
}
