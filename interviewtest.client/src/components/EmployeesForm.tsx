import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import type { EmployeeCreateDto } from "../dtos/EmployeeCreateDto";
import type { EmployeeUpdateDto } from "../dtos/EmployeeUpdateDto";
import type { EmployeeDto } from "../dtos/EmployeeDto";
import "../styles/metricell.css";

// Schema aligned with server-side DataAnnotations
const schema = yup.object({
  name: yup
    .string()
    .required("Name is required")
    .max(100, "Name cannot exceed 100 characters"),
  value: yup
    .number()
    .typeError("Value must be a number")
    .required("Value is required")
    .min(0, "Value must be a non‑negative integer"),
});

interface Props {
  initialValues?: EmployeeDto; // when provided, form is in edit mode
  onSubmit: (
    values: EmployeeCreateDto | EmployeeUpdateDto
  ) => void | Promise<void>;
  onCancel?: () => void;
}

export default function EmployeesForm({
  initialValues,
  onSubmit,
  onCancel,
}: Props) {
  const isEdit = !!initialValues;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<EmployeeCreateDto | EmployeeUpdateDto>({
    resolver: yupResolver(schema),
    defaultValues: { name: "", value: 0 },
    mode: "onBlur",
  });

  // Prefill when editing
  useEffect(() => {
    if (initialValues) {
      reset({ name: initialValues.name, value: initialValues.value });
    } else {
      reset({ name: "", value: 0 });
    }
  }, [initialValues, reset]);

  const submit = async (data: EmployeeCreateDto | EmployeeUpdateDto) => {
    await onSubmit({ name: data.name.trim(), value: Number(data.value) });
  };

  return (
    <form
      onSubmit={handleSubmit(submit)}
      className="card"
      style={{ padding: 12, display: "grid", gap: 10 }}
    >
      <h3 style={{ margin: 0 }}>{isEdit ? "Edit Employee" : "Add Employee"}</h3>

      <label htmlFor="name">
        <div>Name</div>
        <input
          id="name"
          className="input"
          placeholder="e.g. Alice"
          {...register("name")}
          aria-invalid={!!errors.name || undefined}
        />
      </label>
      {errors.name && (
        <div style={{ color: "crimson", marginTop: -6 }}>
          {errors.name.message}
        </div>
      )}

      <label htmlFor="value">
        <div>Value</div>
        <input
          id="value"
          className="input"
          type="number"
          min={0}
          step={1}
          {...register("value", { valueAsNumber: true })}
          aria-invalid={!!errors.value || undefined}
        />
      </label>
      {errors.value && (
        <div style={{ color: "crimson", marginTop: -6 }}>
          {errors.value.message}
        </div>
      )}

      <div style={{ display: "flex", gap: 8 }}>
        <button
          className="btn btn--primary"
          type="submit"
          disabled={isSubmitting}
        >
          {isEdit ? "Save" : "Create"}
        </button>
        {onCancel && (
          <button
            className="btn btn--outline"
            type="button"
            onClick={onCancel}
            disabled={isSubmitting}
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  );
}
