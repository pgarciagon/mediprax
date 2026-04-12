// TextModule Autocomplete — inline #-triggered expansion for textareas
// Uses JS interop with Blazor Server via DotNetObjectReference

const instances = new Map();

/**
 * Initialize autocomplete on a textarea element.
 * @param {object} dotNetRef - DotNetObjectReference for callbacks
 * @param {string} textareaId - HTML id of the textarea element
 */
export function initAutocomplete(dotNetRef, textareaId) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return;

    // Clean up any existing instance
    if (instances.has(textareaId)) {
        dispose(textareaId);
    }

    let debounceTimer = null;
    let currentHashStart = -1; // position of the '#' character

    const onInput = () => {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            const { word, start } = getHashWordAtCursor(textarea);
            if (word && word.length >= 2) {
                currentHashStart = start;
                dotNetRef.invokeMethodAsync('OnTextModuleSearch', word);
            } else {
                currentHashStart = -1;
                dotNetRef.invokeMethodAsync('OnDismissDropdown');
            }
        }, 200);
    };

    const onKeyDown = (e) => {
        if (currentHashStart === -1) return;

        if (e.key === 'ArrowDown') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OnNavigate', 'down');
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OnNavigate', 'up');
        } else if (e.key === 'Enter' && currentHashStart >= 0) {
            // Only intercept Enter when dropdown is visible
            dotNetRef.invokeMethodAsync('OnSelectCurrent').then(selected => {
                if (selected) {
                    e.preventDefault();
                }
            });
        } else if (e.key === 'Escape') {
            currentHashStart = -1;
            dotNetRef.invokeMethodAsync('OnDismissDropdown');
        }
    };

    const onBlur = () => {
        // Delay to allow click on dropdown item
        setTimeout(() => {
            dotNetRef.invokeMethodAsync('OnDismissDropdown');
            currentHashStart = -1;
        }, 200);
    };

    textarea.addEventListener('input', onInput);
    textarea.addEventListener('keydown', onKeyDown);
    textarea.addEventListener('blur', onBlur);

    instances.set(textareaId, { textarea, onInput, onKeyDown, onBlur, dotNetRef });
}

/**
 * Get the caret position in pixels relative to the textarea's parent.
 * Uses a mirror-div technique to measure character positions.
 */
export function getCaretPosition(textareaId) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return { top: 0, left: 0 };

    const computed = window.getComputedStyle(textarea);
    const mirror = document.createElement('div');

    // Copy all relevant styles
    const props = [
        'fontFamily', 'fontSize', 'fontWeight', 'fontStyle', 'letterSpacing',
        'textTransform', 'wordSpacing', 'textIndent', 'whiteSpace',
        'wordWrap', 'overflowWrap', 'lineHeight', 'padding', 'paddingTop',
        'paddingRight', 'paddingBottom', 'paddingLeft', 'borderWidth', 'boxSizing'
    ];
    props.forEach(p => mirror.style[p] = computed[p]);
    mirror.style.position = 'absolute';
    mirror.style.visibility = 'hidden';
    mirror.style.overflow = 'hidden';
    mirror.style.width = computed.width;

    const text = textarea.value.substring(0, textarea.selectionStart);
    mirror.textContent = text;

    // Add a span at cursor position to measure
    const marker = document.createElement('span');
    marker.textContent = '|';
    mirror.appendChild(marker);

    document.body.appendChild(mirror);

    const rect = textarea.getBoundingClientRect();
    const markerRect = marker.getBoundingClientRect();
    const mirrorRect = mirror.getBoundingClientRect();

    const top = markerRect.top - mirrorRect.top - textarea.scrollTop;
    const left = markerRect.left - mirrorRect.left - textarea.scrollLeft;

    document.body.removeChild(mirror);

    return {
        top: Math.min(top + parseInt(computed.lineHeight || '20'), textarea.clientHeight),
        left: Math.min(left, textarea.clientWidth - 200)
    };
}

/**
 * Replace the #shortcut text at the stored position with replacement content.
 */
export function replaceHashWord(textareaId, replacement) {
    const instance = instances.get(textareaId);
    if (!instance) return '';

    const textarea = instance.textarea;
    const { start, end } = getHashWordBounds(textarea);

    if (start < 0) return textarea.value;

    const before = textarea.value.substring(0, start);
    const after = textarea.value.substring(end);
    const newValue = before + replacement + after;

    textarea.value = newValue;

    // Set cursor to end of inserted text
    const cursorPos = start + replacement.length;
    textarea.setSelectionRange(cursorPos, cursorPos);

    // Dispatch events so Blazor picks up the change
    textarea.dispatchEvent(new Event('input', { bubbles: true }));
    textarea.dispatchEvent(new Event('change', { bubbles: true }));

    // Reset state
    instance.dotNetRef.invokeMethodAsync('OnDismissDropdown');

    return newValue;
}

/**
 * Get the current value of the textarea.
 */
export function getValue(textareaId) {
    const textarea = document.getElementById(textareaId);
    return textarea ? textarea.value : '';
}

/**
 * Clean up event listeners for a textarea.
 */
export function dispose(textareaId) {
    const instance = instances.get(textareaId);
    if (!instance) return;

    instance.textarea.removeEventListener('input', instance.onInput);
    instance.textarea.removeEventListener('keydown', instance.onKeyDown);
    instance.textarea.removeEventListener('blur', instance.onBlur);
    instances.delete(textareaId);
}

// --- Helpers ---

/**
 * Extract the word after '#' at the current cursor position.
 * Returns { word, start } where word is the text after '#' and start is the position of '#'.
 */
function getHashWordAtCursor(textarea) {
    const pos = textarea.selectionStart;
    const text = textarea.value;

    // Walk backwards from cursor to find '#'
    let hashPos = -1;
    for (let i = pos - 1; i >= 0; i--) {
        const ch = text[i];
        if (ch === '#') {
            hashPos = i;
            break;
        }
        if (ch === ' ' || ch === '\n' || ch === '\r' || ch === '\t') {
            break; // hit whitespace before finding '#'
        }
    }

    if (hashPos < 0) return { word: null, start: -1 };

    // '#' must be at start of line or preceded by whitespace
    if (hashPos > 0) {
        const before = text[hashPos - 1];
        if (before !== ' ' && before !== '\n' && before !== '\r' && before !== '\t') {
            return { word: null, start: -1 };
        }
    }

    const word = text.substring(hashPos + 1, pos);
    return { word, start: hashPos };
}

/**
 * Get the full bounds of the #word at the cursor (including the # character).
 */
function getHashWordBounds(textarea) {
    const pos = textarea.selectionStart;
    const text = textarea.value;

    let hashPos = -1;
    for (let i = pos - 1; i >= 0; i--) {
        const ch = text[i];
        if (ch === '#') {
            hashPos = i;
            break;
        }
        if (ch === ' ' || ch === '\n' || ch === '\r' || ch === '\t') break;
    }

    if (hashPos < 0) return { start: -1, end: -1 };

    // Extend end past cursor to include rest of word (in case user typed more)
    let end = pos;
    while (end < text.length) {
        const ch = text[end];
        if (ch === ' ' || ch === '\n' || ch === '\r' || ch === '\t') break;
        end++;
    }

    return { start: hashPos, end };
}
