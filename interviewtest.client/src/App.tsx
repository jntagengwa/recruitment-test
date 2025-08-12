import { useEffect, useMemo, useState } from "react";
import { EmployeesApi } from "./api/Employees";
import type { EmployeeDto } from "./dtos/EmployeeDto";
import EmployeesTable from "./components/EmployeesTable";

function App() {
  const [rows, setRows] = useState<EmployeeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>("");

  const [editing, setEditing] = useState<EmployeeDto | null>(null);
  const [name, setName] = useState("");
  const [value, setValue] = useState<number>(0);
  const [formError, setFormError] = useState<string>("");

  const [abcSum, setAbcSum] = useState<number | null>(null);

  const total = useMemo(
    () => rows.reduce((acc, r) => acc + r.value, 0),
    [rows]
  );

  useEffect(() => {
    refresh();
  }, []);

  async function refresh() {
    setLoading(true);
    setError("");
    try {
      const data = await EmployeesApi.list();
      setRows(data);
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : "Failed to load employees";
      setError(msg);
    } finally {
      setLoading(false);
    }
  }

  function beginCreate() {
    setEditing(null);
    setName("");
    setValue(0);
    setFormError("");
  }

  function beginEdit(row: EmployeeDto) {
    setEditing(row);
    setName(row.name);
    setValue(row.value);
    setFormError("");
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setFormError("");
    const trimmed = name.trim();
    if (!trimmed) {
      setFormError("Name is required");
      return;
    }
    if (trimmed.length > 100) {
      setFormError("Name cannot exceed 100 characters");
      return;
    }
    if (value < 0 || !Number.isFinite(value)) {
      setFormError("Value must be a non-negative integer");
      return;
    }

    try {
      if (editing) {
        await EmployeesApi.update(editing.id, { name: trimmed, value });
        setRows((prev) =>
          prev.map((r) =>
            r.id === editing.id ? { ...r, name: trimmed, value } : r
          )
        );
        setEditing(null);
      } else {
        const created = await EmployeesApi.create({ name: trimmed, value });
        setRows((prev) => [...prev, created]);
      }
      setName("");
      setValue(0);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Save failed";
      setFormError(msg);
    }
  }

  async function handleDelete(id: number) {
    if (!confirm("Delete this employee?")) return;
    try {
      await EmployeesApi.remove(id);
      setRows((prev) => prev.filter((r) => r.id !== id));
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Delete failed";
      alert(msg);
    }
  }

  async function runRule() {
    try {
      const res = await EmployeesApi.runRule();
      setAbcSum(res ? res.sumOfABC : null);
      await refresh();
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Operation failed";
      alert(msg);
    }
  }

  return (
    <div
      style={{
        maxWidth: 960,
        margin: "24px auto",
        padding: 16,
        fontFamily:
          "system-ui, -apple-system, Segoe UI, Roboto, Ubuntu, Cantarell, Noto Sans, sans-serif",
      }}
    >
      <header
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: 12,
        }}
      >
        <h2 style={{ margin: 0 }}>Metricell Employees</h2>
        <div style={{ display: "flex", gap: 8 }}>
          <button onClick={beginCreate}>New Employee</button>
          <button onClick={runRule}>Run Increment Rule</button>
          <button onClick={refresh}>Refresh</button>
        </div>
      </header>

      {abcSum !== null && (
        <div
          style={{
            background: "#eef9f0",
            border: "1px solid #b0e6bd",
            color: "#155724",
            padding: 10,
            borderRadius: 6,
            marginBottom: 12,
          }}
        >
          Sum of A/B/C is <strong>{abcSum.toLocaleString()}</strong>
        </div>
      )}

      {error && (
        <div
          style={{
            background: "#fdecea",
            border: "1px solid #f5c6cb",
            color: "#721c24",
            padding: 10,
            borderRadius: 6,
            marginBottom: 12,
          }}
        >
          {error}
        </div>
      )}

      <section
        style={{
          display: "grid",
          gridTemplateColumns: "minmax(280px, 360px) 1fr",
          gap: 12,
          alignItems: "start",
          marginBottom: 16,
        }}
      >
        <form
          onSubmit={handleSubmit}
          className="card"
          style={{ padding: 12, display: "grid", gap: 8 }}
        >
          <h3 style={{ margin: 0 }}>
            {editing ? `Edit #${editing.id}` : "Add Employee"}
          </h3>

          <label htmlFor="name">
            <div>Name</div>
            <input
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Alice"
            />
          </label>

          <label htmlFor="value">
            <div>Value</div>
            <input
              id="value"
              type="number"
              min={0}
              value={value}
              onChange={(e) => setValue(Number(e.target.value))}
            />
          </label>

          {formError && <div style={{ color: "crimson" }}>{formError}</div>}

          <div style={{ display: "flex", gap: 8 }}>
            <button type="submit">{editing ? "Save" : "Create"}</button>
            {editing && (
              <button
                type="button"
                onClick={() => {
                  setEditing(null);
                  setName("");
                  setValue(0);
                }}
              >
                Cancel
              </button>
            )}
          </div>
        </form>

        <div>
          <div className="card" style={{ padding: 12, marginBottom: 12 }}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <div style={{ color: "#666" }}>
                Total Value: <strong>{total.toLocaleString()}</strong>
              </div>
              <div style={{ color: "#666" }}>
                Count: <strong>{rows.length}</strong>
              </div>
            </div>
          </div>

          {loading ? (
            <div>Loading…</div>
          ) : (
            <EmployeesTable
              rows={rows}
              onEdit={beginEdit}
              onDelete={handleDelete}
            />
          )}
        </div>
      </section>
    </div>
  );
}

export default App;
