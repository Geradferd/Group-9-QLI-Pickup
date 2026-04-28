import { useState, useEffect } from "react";
import { apiCall, brand, font, type TransportationType, type RiderResponse, type User } from "../lib/api";
import NewTripFormBody from "./NewTripFormBody";

interface Props {
  user: User;
  onClose: () => void;
  onCreated: () => void;
}

export default function NewTripModal({ user, onClose, onCreated }: Props) {
  const [transportationTypes, setTransportationTypes] = useState<TransportationType[]>([]);
  const [riders, setRiders] = useState<RiderResponse[]>([]);
  const isAdmin = user.role?.toLowerCase() === "admin";

  const [riderId, setRiderId] = useState("");
  const [transportationTypeId, setTransportationTypeId] = useState("");
  const [pickupAddress, setPickupAddress] = useState("");
  const [destinationAddress, setDestinationAddress] = useState("");
  const [scheduledPickupTime, setScheduledPickupTime] = useState("");
  const [passengerCount, setPassengerCount] = useState("1");
  const [requiresWheelchair, setRequiresWheelchair] = useState(false);
  const [notes, setNotes] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [apiError, setApiError] = useState("");
  const [focused, setFocused] = useState<Record<string, boolean>>({});

  const focusProps = (name: string) => ({
    onFocus: () => setFocused(p => ({ ...p, [name]: true })),
    onBlur: () => setFocused(p => ({ ...p, [name]: false })),
  });

  useEffect(() => {
    apiCall("/transportationtypes", "GET", null, user.token)
      .then(data => setTransportationTypes((data ?? []).filter((t: TransportationType) => t.isActive)))
      .catch(() => {});
    if (isAdmin) {
      apiCall("/rider", "GET", null, user.token).then(data => setRiders(data ?? [])).catch(() => {});
    }
    const d = new Date();
    d.setHours(d.getHours() + 1);
    d.setMinutes(Math.ceil(d.getMinutes() / 15) * 15, 0, 0);
    setScheduledPickupTime(new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16));
  }, []);

  function validate() {
    const e: Record<string, string> = {};
    if (!transportationTypeId) e.transportationTypeId = "Please select a transportation type.";
    if (!pickupAddress.trim()) e.pickupAddress = "Pickup address is required.";
    if (!destinationAddress.trim()) e.destinationAddress = "Destination address is required.";
    if (!scheduledPickupTime) e.scheduledPickupTime = "Please select a pickup date and time.";
    const pax = parseInt(passengerCount);
    if (isNaN(pax) || pax < 1 || pax > 20) e.passengerCount = "Passenger count must be 1–20.";
    return e;
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }
    setErrors({});
    setApiError("");
    setSubmitting(true);
    try {
      await apiCall("/trips", "POST", {
        riderId: riderId ? parseInt(riderId) : null,
        transportationTypeId: parseInt(transportationTypeId),
        pickupAddress: pickupAddress.trim(),
        destinationAddress: destinationAddress.trim(),
        scheduledPickupTime: scheduledPickupTime + ":00",
        passengerCount: parseInt(passengerCount),
        requiresWheelchair,
        notes: notes.trim() || null,
      }, user.token);
      onCreated();
      onClose();
    } catch (err: unknown) {
      setApiError(err instanceof Error ? err.message : "Failed to create trip.");
    }
    setSubmitting(false);
  }

  return (
    <div onClick={onClose} style={{ position: "fixed", inset: 0, background: "rgba(0,0,0,0.45)", zIndex: 1000, display: "flex", alignItems: "center", justifyContent: "center", padding: 24 }}>
      <div onClick={e => e.stopPropagation()} style={{ background: brand.white, borderRadius: 16, width: "100%", maxWidth: 560, maxHeight: "90vh", display: "flex", flexDirection: "column", boxShadow: "0 20px 60px rgba(0,0,0,0.25)", overflow: "hidden" }}>
        {/* Header */}
        <div style={{ padding: "20px 24px", borderBottom: `1px solid ${brand.borderGray}`, display: "flex", justifyContent: "space-between", alignItems: "center", flexShrink: 0 }}>
          <div>
            <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, color: brand.charcoal, fontFamily: font }}>New Trip Request</h2>
            <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>Fill in the details below to schedule a trip.</p>
          </div>
          <button onClick={onClose} style={{ width: 32, height: 32, borderRadius: "50%", border: `1px solid ${brand.borderGray}`, background: brand.white, cursor: "pointer", display: "flex", alignItems: "center", justifyContent: "center", fontSize: 18, color: brand.medGray }}>×</button>
        </div>
        {/* Body */}
        <form onSubmit={handleSubmit} style={{ overflowY: "auto", flex: 1, padding: "24px" }}>
          <NewTripFormBody
            state={{ riderId, transportationTypeId, pickupAddress, destinationAddress, scheduledPickupTime, passengerCount, requiresWheelchair, notes }}
            setters={{ setRiderId, setTransportationTypeId, setPickupAddress, setDestinationAddress, setScheduledPickupTime, setPassengerCount, setRequiresWheelchair, setNotes }}
            errors={errors}
            apiError={apiError}
            submitting={submitting}
            isAdmin={isAdmin}
            riders={riders}
            transportationTypes={transportationTypes}
            focused={focused}
            focusProps={focusProps}
            onClose={onClose}
          />
        </form>
      </div>
    </div>
  );
}
