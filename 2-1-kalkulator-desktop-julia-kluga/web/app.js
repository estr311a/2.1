document.addEventListener("DOMContentLoaded", () => {
  const mainDisplay = document.getElementById("mainDisplay");
  const exprDisplay = document.getElementById("exprDisplay");
  const memoryBadge = document.getElementById("memoryBadge");
  const historyList = document.getElementById("historyList");
  const historyPanel = document.getElementById("historyPanel");
  const btnClearHistory = document.getElementById("btnClearHistory");
  const btnToggleHistory = document.getElementById("btnToggleHistory");

  let currentValue = "0";
  let pendingOperand = null;
  let pendingOperator = null;
  let isNewEntry = true;
  let memoryValue = 0;
  let hasMemory = false;
  let history = [];

  function updateDisplay() {
    mainDisplay.textContent = currentValue;
    if (pendingOperator && pendingOperand !== null) {
      exprDisplay.textContent = `${pendingOperand} ${getOpSymbol(pendingOperator)}`;
    } else {
      exprDisplay.textContent = "";
    }
    memoryBadge.textContent = hasMemory ? `M (${memoryValue})` : "";
  }

  function getOpSymbol(op) {
    switch (op) {
      case "add": return "+";
      case "sub": return "−";
      case "mul": return "×";
      case "div": return "÷";
      case "percent": return "%";
      default: return "";
    }
  }

  function inputDigit(digit) {
    if (isNewEntry || currentValue === "0") {
      currentValue = digit;
      isNewEntry = false;
    } else {
      if (currentValue.length < 16) {
        currentValue += digit;
      }
    }
    updateDisplay();
  }

  function inputDot() {
    if (isNewEntry) {
      currentValue = "0.";
      isNewEntry = false;
    } else if (!currentValue.includes(".")) {
      currentValue += ".";
    }
    updateDisplay();
  }

  function setOperator(op) {
    const val = parseFloat(currentValue);
    if (pendingOperator && !isNewEntry) {
      calculateResult();
    } else {
      pendingOperand = val;
    }
    pendingOperator = op;
    isNewEntry = true;
    exprDisplay.textContent = `${pendingOperand} ${getOpSymbol(op)}`;
  }

  function calculateResult() {
    if (!pendingOperator || pendingOperand === null) return;

    const current = parseFloat(currentValue);
    let result = 0;
    const opSym = getOpSymbol(pendingOperator);
    const exprText = `${pendingOperand} ${opSym} ${current}`;

    switch (pendingOperator) {
      case "add": result = pendingOperand + current; break;
      case "sub": result = pendingOperand - current; break;
      case "mul": result = pendingOperand * current; break;
      case "div":
        if (Math.abs(current) < 1e-10) {
          currentValue = "Błąd: dzielenie przez 0";
          pendingOperator = null;
          pendingOperand = null;
          isNewEntry = true;
          updateDisplay();
          return;
        }
        result = pendingOperand / current;
        break;
      case "percent":
        result = (pendingOperand * current) / 100;
        break;
    }

    result = Math.round(result * 1e10) / 1e10;
    const resStr = result.toString();
    addHistory(exprText, resStr);

    currentValue = resStr;
    pendingOperator = null;
    pendingOperand = null;
    isNewEntry = true;
    exprDisplay.textContent = `${exprText} =`;
    mainDisplay.textContent = currentValue;
  }

  function applyUnary(action) {
    const val = parseFloat(currentValue);
    let result = 0;
    let desc = "";

    if (action === "sqrt") {
      if (val < 0) {
        currentValue = "Zła wartość";
        isNewEntry = true;
        updateDisplay();
        return;
      }
      result = Math.sqrt(val);
      desc = `√(${val})`;
    } else if (action === "sqr") {
      result = val * val;
      desc = `sqr(${val})`;
    } else if (action === "recip") {
      if (Math.abs(val) < 1e-10) {
        currentValue = "Nie dziel przez 0";
        isNewEntry = true;
        updateDisplay();
        return;
      }
      result = 1 / val;
      desc = `1/(${val})`;
    }

    result = Math.round(result * 1e10) / 1e10;
    const resStr = result.toString();
    addHistory(desc, resStr);
    currentValue = resStr;
    isNewEntry = true;
    exprDisplay.textContent = `${desc} =`;
    mainDisplay.textContent = currentValue;
  }

  function addHistory(expr, res) {
    history.unshift({ expr, res });
    renderHistory();
  }

  function renderHistory() {
    if (history.length === 0) {
      historyList.innerHTML = '<div class="empty-hint">Brak wcześniejszych obliczeń</div>';
      return;
    }
    historyList.innerHTML = "";
    history.forEach(item => {
      const div = document.createElement("div");
      div.className = "history-item";
      div.innerHTML = `<div class="hist-expr">${item.expr} =</div><div class="hist-res">${item.res}</div>`;
      div.addEventListener("click", () => {
        currentValue = item.res;
        isNewEntry = true;
        updateDisplay();
      });
      historyList.appendChild(div);
    });
  }

  btnClearHistory.addEventListener("click", () => {
    history = [];
    renderHistory();
  });

  btnToggleHistory.addEventListener("click", () => {
    historyPanel.style.display = (historyPanel.style.display === "none") ? "flex" : "none";
  });

  document.querySelectorAll(".keypad button").forEach(btn => {
    btn.addEventListener("click", () => {
      const num = btn.getAttribute("data-num");
      const action = btn.getAttribute("data-action");

      if (num !== null) {
        inputDigit(num);
      } else if (action) {
        switch (action) {
          case "dot": inputDot(); break;
          case "add": case "sub": case "mul": case "div": case "percent":
            setOperator(action);
            break;
          case "equals": calculateResult(); break;
          case "c":
            currentValue = "0";
            pendingOperator = null;
            pendingOperand = null;
            isNewEntry = true;
            updateDisplay();
            break;
          case "ce":
            currentValue = "0";
            isNewEntry = true;
            updateDisplay();
            break;
          case "backspace":
            if (!isNewEntry && currentValue.length > 1) {
              currentValue = currentValue.slice(0, -1);
            } else {
              currentValue = "0";
              isNewEntry = true;
            }
            updateDisplay();
            break;
          case "negate":
            currentValue = (parseFloat(currentValue) * -1).toString();
            updateDisplay();
            break;
          case "sqrt": case "sqr": case "recip":
            applyUnary(action);
            break;
          case "mc":
            memoryValue = 0;
            hasMemory = false;
            updateDisplay();
            break;
          case "mr":
            if (hasMemory) {
              currentValue = memoryValue.toString();
              isNewEntry = true;
              updateDisplay();
            }
            break;
          case "mplus":
            memoryValue += parseFloat(currentValue) || 0;
            hasMemory = true;
            updateDisplay();
            break;
          case "mminus":
            memoryValue -= parseFloat(currentValue) || 0;
            hasMemory = true;
            updateDisplay();
            break;
        }
      }
    });
  });

  // Klawiatura fizyczna
  window.addEventListener("keydown", (e) => {
    if (e.key >= "0" && e.key <= "9") inputDigit(e.key);
    else if (e.key === "." || e.key === ",") inputDot();
    else if (e.key === "+") setOperator("add");
    else if (e.key === "-") setOperator("sub");
    else if (e.key === "*") setOperator("mul");
    else if (e.key === "/") { e.preventDefault(); setOperator("div"); }
    else if (e.key === "%") setOperator("percent");
    else if (e.key === "Enter" || e.key === "=") { e.preventDefault(); calculateResult(); }
    else if (e.key === "Backspace") { e.preventDefault(); document.querySelector('[data-action="backspace"]').click(); }
    else if (e.key === "Escape") document.querySelector('[data-action="c"]').click();
  });

  updateDisplay();
});
