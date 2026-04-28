import { useState } from "react";
import { apiCall, brand, font, statusLabel, type TripResponse, type User } from "../lib/api";

type Action = "accept" | "start" | "complete" | "cancel";

interface Props {
  trip: TripResponse;
  user: User;
  onClose: () => void;
  onDone: () => void;
}

const actionMeta: Record<Action, { label: string; color: string; bg: string; needsReason: boolean; reasonLabel: string; icon: string }> = {
  accept:    { label: "Accept Ride",      color: "#0891b2", bg: "#ecfeff", needsReason: false, reasonLabel: "",                                   icon: "👍" },
  start:     { label: "Start Trip",       color: "#2563eb", bg: "#eff6ff", needsReason: false, reasonLabel: "",                                   icon: "🚦" },
  complete:  { label: "Mark Completed",   color: "#7c3aed", bg: "#f5f3ff", needsReason: false, reasonLabel: "",                                   icon: "🏁" },
  cancel:    { label: "Cancel Ride",      color: "#ea580c", bg: "#fff7ed", needsReason: true,  reasonLabel: "Reason for cancellation (optional)", icon: "🚫" },
};

export default function TripActionsModal({ trip, user, onClose, onDone }: Props) {
  const [action, setAction] = useState<Action | null>(null);
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const status = trip.status?.toLowerCase() ?? "";
  const role = user.role?.toLowerCase() ?? "";
  const isAdmin = role === "admin";
  const isRider = role === "rider";
  const isDriver = role === "driver";

  // When a rider cancels an authorized (awaiting acceptance) trip, call it "Deny"
  const riderDenying = isRider && status === "authorized";

  const getActionLabel = (a: Action) => {
    if (a === "cancel" && riderDenying) return "Deny Ride";
    return actionMeta[a].label;
  };
  const getActionIcon = (a: Action) => {
    if (a === "cancel" && riderDenying) return "❌";
    return actionMeta[a].icon;
  };
  const getActionColor = (a: Action) => {
    if (a === "cancel" && riderDenying) return "#dc2626";
    return actionMeta[a].color;
  };
  const getActionBg = (a: Action) => {
    if (a === "cancel" && riderDenying) return "#fef2f2";
    return actionMeta[a].bg;
  };

  // Status flow: Awaiting Rider (Authorized) → Accepted (rider accepts) or Cancelled
  //              Accepted → InProgress → Completed (admin/driver)
  const availableActions: Action[] = [];
  if (status === "pending") {
    if (isAdmin) availableActions.push("cancel");
  }
  if (status === "authorized") {
    if (isRider) availableActions.push("accept", "cancel");
    if (isAdmin) availableActions.push("cancel");
  }
  if (status === "accepted") {
    if (isAdmin || isDriver) availableActions.push("start");
    if (isAdmin || isRider) availableActions.push("cancel");
  }
  if (status === "scheduled") {
    if (isAdmin || isDriver) availableActions.push("start", "cancel");
  }
  if (status === "inprogress") {
    if (isAdmin || isDriver) availableActions.push("complete");
  }

  const meta = action ? actionMeta[action] : null;

  const handleSubmit = async () => {
    if (!action) return;
    if (actionMeta[action].needsReason && action === "cancel" && !reason.trim()) {
      setError("Please provide a reason for cancellation.");
      return;
    }
    setLoading(true);
    setError("");
    try {
      let body: object | null = null;
      if (action === "cancel") body = { reason: reason.trim() };
      else if (action === "complete") body = {};
      await apiCall(`/trips/${trip.id}/${action}`, "POST", body, user.token);
      onDone();
      onClose();
    } catch (e: any) {
      setError(e?.message ?? "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      position: "fixed", inset: 0, zIndex: 1000,
      background: "rgba(0,0,0,0.45)", backdropFilter: "blur(3px)",
      display: "flex", alignItems: "center", justifyContent: "center", padding: 24,
    }} onClick={e => { if (e.target === e.currentTarget) onClose(); }}>
      <div style={{
        background: brand.white, borderRadius: 16, width: "100%", maxWidth: 480,
        boxShadow: "0 20px 60px rgba(0,0,0,0.2)", overflow: "hidden",
      }}>
        {/* Header */}
        <div style={{
          padding: "20px 24px", borderBottom: `1px solid ${brand.borderGray}`,
          display: "flex", justifyContent: "space-between", alignItems: "center",
        }}>
          <div>
            <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, color: brand.charcoal, fontFamily: font }}>
              Manage Trip #{trip.id}
            </h2>
            <p style={{ margin: "4px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>
              {trip.pickupAddress} → {trip.destinationAddress}
            </p>
          </div>
          <button onClick={onClose} style={{
            background: "transparent", border: "none", cursor: "pointer",
            fontSize: 22, color: brand.medGray, lineHeight: 1, padding: 4,
          }}>×</button>
        </div>

        <div style={{ padding: 24 }}>
          {/* Trip info */}
          <div style={{
            background: brand.lightGray, borderRadius: 10, padding: "12px 16px",
            marginBottom: 20, display: "grid", gridTemplateColumns: "1fr 1fr", gap: "8px 16px",
            fontSize: 13, fontFamily: font,
          }}>
            <div><span style={{ color: brand.medGray }}>Status: </span><strong style={{ color: brand.charcoal }}>{statusLabel(trip.status)}</strong></div>
            <div><span style={{ color: brand.medGray }}>Passengers: </span><strong style={{ color: brand.charcoal }}>{trip.passengerCount}</strong></div>
            <div style={{ gridColumn: "1 / -1" }}>
              <span style={{ color: brand.medGray }}>Pickup: </span>
              <strong style={{ color: brand.charcoal }}>
                {new Date(trip.scheduledPickupTime).toLocaleString([], { weekday: "short", month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" })}
              </strong>
            </div>
            {trip.notes && (
              <div style={{ gridColumn: "1 / -1" }}>
                <span style={{ color: brand.medGray }}>Notes: </span>
                <span style={{ color: brand.charcoal }}>{trip.notes}</span>
              </div>
            )}
          </div>

          {availableActions.length === 0 ? (
            <p style={{ color: brand.medGray, fontSize: 14, fontFamily: font, textAlign: "center", padding: "16px 0" }}>
              No actions available for a <strong>{statusLabel(trip.status)}</strong> trip.
            </p>
          ) : !action ? (
            <>
              <p style={{ margin: "0 0 14px", fontSize: 14, color: brand.darkGray, fontFamily: font, fontWeight: 500 }}>
                Choose an action:
              </p>
              <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
                {availableActions.map(a => (
                    <button key={a} onClick={() => setAction(a)} style={{
                      padding: "13px 18px", borderRadius: 10, cursor: "pointer",
                      border: `1.5px solid ${getActionColor(a)}30`, background: getActionBg(a),
                      color: getActionColor(a), fontWeight: 600, fontSize: 14, fontFamily: font,
                      textAlign: "left", display: "flex", alignItems: "center", gap: 10,
                      transition: "filter 0.15s",
                    }}
                    onMouseEnter={e => (e.currentTarget.style.filter = "brightness(0.96)")}
                    onMouseLeave={e => (e.currentTarget.style.filter = "none")}
                    >
                      <span style={{ fontSize: 20 }}>{getActionIcon(a)}</span>
                      {getActionLabel(a)}
                    </button>
                ))}
              </div>
            </>
          ) : (
            <>
              <button onClick={() => { setAction(null); setReason(""); setError(""); }} style={{
                background: "transparent", border: "none", cursor: "pointer",
                color: brand.blue, fontSize: 13, fontFamily: font, padding: "0 0 14px",
                display: "flex", alignItems: "center", gap: 4,
              }}>← Back</button>

              <div style={{
                padding: "14px 16px", borderRadius: 10, background: getActionBg(action),
                border: `1.5px solid ${getActionColor(action)}30`, marginBottom: 18,
                fontSize: 14, fontWeight: 600, color: getActionColor(action), fontFamily: font,
              }}>
                {getActionLabel(action)}
              </div>

              {meta!.needsReason && (
                <div style={{ marginBottom: 18 }}>
                  <label style={{ display: "block", fontSize: 13, fontWeight: 500, color: brand.darkGray, marginBottom: 6, fontFamily: font }}>
                    {meta!.reasonLabel}
                  </label>
                  <textarea
                    rows={3}
                    value={reason}
                    onChange={e => setReason(e.target.value)}
                    placeholder="Enter reason…"
                    style={{
                      width: "100%", padding: "10px 12px", fontSize: 14, fontFamily: font,
                      border: `1.5px solid ${brand.borderGray}`, borderRadius: 8,
                      resize: "vertical", outline: "none", color: brand.charcoal,
                    }}
                    onFocus={e => (e.target.style.borderColor = brand.blue)}
                    onBlur={e => (e.target.style.borderColor = brand.borderGray)}
                  />
                </div>
              )}

              {error && (
                <div style={{
                  padding: "10px 14px", borderRadius: 8, background: "#fef2f2",
                  border: "1px solid #fca5a5", color: "#dc2626", fontSize: 13,
                  fontFamily: font, marginBottom: 16,
                }}>{error}</div>
              )}

              <div style={{ display: "flex", gap: 10 }}>
                <button onClick={onClose} disabled={loading} style={{
                  flex: 1, padding: "11px", borderRadius: 8, border: `1px solid ${brand.borderGray}`,
                  background: brand.white, color: brand.darkGray, fontSize: 14,
                  fontWeight: 500, cursor: "pointer", fontFamily: font,
                }}>Cancel</button>
                <button onClick={handleSubmit} disabled={loading} style={{
                  flex: 2, padding: "11px", borderRadius: 8, border: "none",
                  background: getActionColor(action), color: brand.white, fontSize: 14,
                  fontWeight: 600, cursor: loading ? "not-allowed" : "pointer",
                  fontFamily: font, opacity: loading ? 0.7 : 1,
                }}>
                  {loading ? "Saving…" : getActionLabel(action)}
                </button>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
