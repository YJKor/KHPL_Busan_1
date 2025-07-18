using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SingleGrabOnly : XRGrabInteractable
{
    private IXRSelectInteractor currentInteractor = null;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // �� �ո� ��� �ϱ� ���� ���� interactor ����
        currentInteractor = args.interactorObject;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Grab ���� �� interactor �ʱ�ȭ
        currentInteractor = null;
        base.OnSelectExited(args);
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // �̹� �ٸ� ���� ��� ������ ����
        if (currentInteractor != null && currentInteractor != interactor)
            return false;

        return base.IsSelectableBy(interactor);
    }
}