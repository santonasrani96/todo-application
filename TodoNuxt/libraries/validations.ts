export const formInvalid = (params: any): boolean => {
  if (typeof params === "object") {
    for (const key in params) {
      if (
        params[key] === null ||
        params[key] === "" ||
        params[key] === 0 ||
        params[key] === "0" ||
        params[key] <= 0 ||
        params[key] === undefined
      ) {
        return true;
      }
    }
  }

  return false;
};

const validationRules: any = {
  required(value: string | number): boolean {
    return value !== null && value !== undefined && value !== "";
  },
  numeric(value: number): boolean {
    return !isNaN(value);
  },
  isEmail(value: string): boolean {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(value);
  },
  isPhoneNumber(value: string): boolean {
    const phonePattern = /^(\+|0)[0-9]{6,20}$/;
    return phonePattern.test(value);
  },
  max(value: string | number, params: { max: string | number }): boolean {
    return Number(value) <= Number(params.max);
  },
  min(value: string | number, params: { min: string | number }): boolean {
    return Number(value) <= Number(params.min);
  },
  maxLength(value: string, params: { max: string | number }): boolean {
    return value.replace(/\s/g, "").length <= Number(params.max);
  },
  minLength(value: string, params: { min: string | number }): boolean {
    return value.replace(/\s/g, "").length >= Number(params.min);
  },
  isGreaterThanZero(value: string): boolean {
    return Number(value) > 0 && /^[1-9]\d*$/.test(value);
  },
  isNotPastDate(value: string): boolean {
    const inputDate = new Date(value);
    const currentDate = new Date();
    currentDate.setHours(0, 0, 0, 0);
    return inputDate >= currentDate;
  },
  isMatch(value: string, params: { match_to: string }): boolean {
    return value === params.match_to;
  },
  hasNumber(value: string): boolean {
    const numberPattern = /\d/;
    return numberPattern.test(value);
  },
  hasSymbol(value: string): boolean {
    const symbolPattern = /[!@#$%^&*(),.?":{}|<>]/;
    return symbolPattern.test(value);
  },
  hasUpperCase(value: string): boolean {
    const upperCasePattern = /[A-Z]/;
    return upperCasePattern.test(value);
  },
};

export function validate(
  value: string | number,
  rules: any[]
): { ruleName: string | null; valid: boolean } {
  for (const rule of rules) {
    const ruletype = rule.params !== undefined ? "function" : "string";

    const rulename = rule.name;
    const params = ruletype === "function" ? rule.params : null;
    let isValid = false;

    if (params) {
      isValid = validationRules[rulename](value, params);
    } else {
      isValid = validationRules[rulename](value);
    }

    if (!isValid) {
      return {
        ruleName: rulename,
        valid: isValid,
      };
    }
  }

  return {
    ruleName: null,
    valid: true,
  };
}
