import re

with open('manager.html', 'r', encoding='utf-8') as f:
    text = f.read()

# 1. Add CSS
text = text.replace('.sidebar-title-left {', '.node-content.selected { background: #e8f0fe !important; border-left: 3px solid #1a73e8; }\n        .node-content.selected .node-name { font-weight: 500; color: #1a73e8; }\n        .sidebar-title-left {')

# 2. Update variables
text = text.replace(
    'let treeData = [];\n        let draggedItem = null;',
    'let treeData = [];\n        let selectedNodes = new Set();\n        let draggedItems = [];'
)

# 3. Update nodeName click logic
text = text.replace(
    '<span class="node-name" onclick="openFile(\'${node.id}\')">${node.text}</span>',
    '<span class="node-name">${node.text}</span>'
)

# 4. Add onclick to nodeContent
# We need to find: nodeContent.className = 'node-content' + (activeFile === node.id ? ' active' : '');
# And add nodeContent.onclick to it.
replace_render = """                nodeContent.className = 'node-content' + (activeFile === node.id ? ' active' : '') + (selectedNodes.has(node.id) ? ' selected' : '');
                
                nodeContent.onclick = (e) => {
                    if (e.target.closest('.actions') || e.target.classList.contains('toggle-icon') || e.target.classList.contains('drag-handle')) return;
                    if (e.shiftKey || e.metaKey || e.ctrlKey) {
                        if (selectedNodes.has(node.id)) selectedNodes.delete(node.id);
                        else selectedNodes.add(node.id);
                    } else {
                        selectedNodes.clear();
                        selectedNodes.add(node.id);
                        openFile(node.id);
                    }
                    renderAll();
                };"""
text = text.replace("                nodeContent.className = 'node-content' + (activeFile === node.id ? ' active' : '');", replace_render)

# 5. Drag start
drag_start_old = """                handle.ondragstart = (e) => {
                    draggedItem = node;
                    dropZone = null;
                    e.dataTransfer.effectAllowed = 'move';
                    e.dataTransfer.setData('text/plain', node.id);
                    setTimeout(() => nodeContent.style.opacity = '0.4', 0);
                };"""
drag_start_new = """                handle.ondragstart = (e) => {
                    if (!selectedNodes.has(node.id)) {
                        selectedNodes.clear();
                        selectedNodes.add(node.id);
                        document.querySelectorAll('.node-content').forEach(el => el.classList.remove('selected'));
                        nodeContent.classList.add('selected');
                    }
                    draggedItems = Array.from(selectedNodes).map(id => findNode(treeData, id)).filter(x => x);
                    dropZone = null;
                    e.dataTransfer.effectAllowed = 'move';
                    e.dataTransfer.setData('text/plain', 'multi');
                    setTimeout(() => document.querySelectorAll('.node-content.selected').forEach(el => el.style.opacity = '0.4'), 0);
                };"""
text = text.replace(drag_start_old, drag_start_new)

# 6. Drag end
drag_end_old = """                handle.ondragend = (e) => {
                    nodeContent.style.opacity = '';
                    document.querySelectorAll('.node-content').forEach(el => {
                        el.classList.remove('drag-over', 'drag-above', 'drag-below');
                    });
                    draggedItem = null;
                    dropZone = null;
                };"""
drag_end_new = """                handle.ondragend = (e) => {
                    document.querySelectorAll('.node-content.selected').forEach(el => el.style.opacity = '');
                    document.querySelectorAll('.node-content').forEach(el => {
                        el.classList.remove('drag-over', 'drag-above', 'drag-below');
                    });
                    draggedItems = [];
                    dropZone = null;
                };"""
text = text.replace(drag_end_old, drag_end_new)

# 7. Drag over
text = text.replace(
    'if (!draggedItem || draggedItem.id === node.id) return;',
    'if (!draggedItems || draggedItems.length === 0 || draggedItems.find(x => x.id === node.id)) return;'
)

# 8. Drop
drop_old = """                nodeContent.ondrop = (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    nodeContent.classList.remove('drag-over', 'drag-above', 'drag-below');
                    if (draggedItem && draggedItem.id !== node.id && dropZone) {
                        // Ensure target node has children array for 'inside'
                        if (dropZone === 'inside' && !node.children) node.children = [];
                        moveNode(treeData, draggedItem.id, node.id, dropZone);
                        draggedItem = null;
                        dropZone = null;
                        saveTree();
                        renderAll();
                    }
                };"""
drop_new = """                nodeContent.ondrop = (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    nodeContent.classList.remove('drag-over', 'drag-above', 'drag-below');
                    if (draggedItems.length > 0 && !draggedItems.find(x => x.id === node.id) && dropZone) {
                        if (dropZone === 'inside' && !node.children) node.children = [];
                        moveNodes(treeData, draggedItems.map(x => x.id), node.id, dropZone);
                        draggedItems = [];
                        dropZone = null;
                        saveTree();
                        renderAll();
                    }
                };"""
text = text.replace(drop_old, drop_new)

# Root drop
root_drop_old = """                container.ondrop = (e) => {
                    e.preventDefault();
                    if (draggedItem) {
                        const isTop = e.clientY < 180;
                        moveNode(treeData, draggedItem.id, null, isTop ? 'top' : 'bottom');
                        saveTree();
                        renderAll();
                    }
                };"""
root_drop_new = """                container.ondrop = (e) => {
                    e.preventDefault();
                    if (draggedItems.length > 0) {
                        const isTop = e.clientY < 180;
                        moveNodes(treeData, draggedItems.map(x => x.id), null, isTop ? 'top' : 'bottom');
                        saveTree();
                        renderAll();
                    }
                };"""
text = text.replace(root_drop_old, root_drop_new)

# 9. moveNode -> moveNodes
move_code_old = """        function moveNode(tree, draggedId, targetId, position = 'inside') {
            let extracted = removeNode(tree, draggedId);
            if (!extracted) return;

            if (targetId === null) {
                if (position === 'top') {
                    // Prepend to top
                    tree.unshift(extracted);
                } else {
                    tree.push(extracted);
                }
            } else {
                insertNodePosition(tree, targetId, extracted, position);
            }
        }"""
move_code_new = """        function moveNodes(tree, draggedIds, targetId, position = 'inside') {
            let extractedNodes = [];
            for (let id of draggedIds) {
                let ex = removeNode(tree, id);
                if (ex) extractedNodes.push(ex);
            }
            if (extractedNodes.length === 0) return;

            if (targetId === null) {
                if (position === 'top') {
                    tree.unshift(...extractedNodes);
                } else {
                    tree.push(...extractedNodes);
                }
            } else {
                insertNodesPosition(tree, targetId, extractedNodes, position);
            }
        }"""
text = text.replace(move_code_old, move_code_new)

insert_code_old = """        function insertNodePosition(nodes, targetId, movingNode, position) {
            for (let i = 0; i < nodes.length; i++) {
                if (nodes[i].id === targetId) {
                    if (position === 'inside') {
                        nodes[i].children = nodes[i].children || [];
                        nodes[i].children.push(movingNode);
                    } else if (position === 'before') {
                        nodes.splice(i, 0, movingNode);
                    } else if (position === 'after') {
                        nodes.splice(i + 1, 0, movingNode);
                    }
                    return true;
                }
                if (nodes[i].children) {
                    if (insertNodePosition(nodes[i].children, targetId, movingNode, position)) return true;
                }
            }
            return false;
        }"""
insert_code_new = """        function insertNodesPosition(nodes, targetId, movingNodes, position) {
            for (let i = 0; i < nodes.length; i++) {
                if (nodes[i].id === targetId) {
                    if (position === 'inside') {
                        nodes[i].children = nodes[i].children || [];
                        nodes[i].children.push(...movingNodes);
                    } else if (position === 'before') {
                        nodes.splice(i, 0, ...movingNodes);
                    } else if (position === 'after') {
                        nodes.splice(i + 1, 0, ...movingNodes);
                    }
                    return true;
                }
                if (nodes[i].children) {
                    if (insertNodesPosition(nodes[i].children, targetId, movingNodes, position)) return true;
                }
            }
            return false;
        }"""
text = text.replace(insert_code_old, insert_code_new)

with open('manager.html', 'w', encoding='utf-8') as f:
    f.write(text)

