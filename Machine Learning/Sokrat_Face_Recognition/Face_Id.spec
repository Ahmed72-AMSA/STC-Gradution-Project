# -*- mode: python ; coding: utf-8 -*-


block_cipher = None


a = Analysis(
    ['Face_Id.py'],
    pathex=[],
    binaries=[],
    datas=[('D:/studying section/projects/GP/STC-Gradution-Project/Machine Learning/Sokrat_Face_Recognition/siamesemodel.h5', '.'), ('D:/studying section/projects/GP/STC-Gradution-Project/Machine Learning/Sokrat_Face_Recognition/application_data', 'application_data'), ('D:/studying section/projects/GP/STC-Gradution-Project/Machine Learning/Sokrat_Face_Recognition/application_data/verification_images', 'verification_images')],
    hiddenimports=['tensorflow', 'numpy'],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)
pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.zipfiles,
    a.datas,
    [],
    name='Face_Id',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
