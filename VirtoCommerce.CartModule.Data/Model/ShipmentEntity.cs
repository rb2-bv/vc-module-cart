using Omu.ValueInjecter;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShipmentEntity : AuditableEntity
    {
        [StringLength(64)]
        public string ShipmentMethodCode { get; set; }

        [StringLength(64)]
        public string ShipmentMethodOption { get; set; }

        [StringLength(64)]
        public string FulfilmentCenterId { get; set; }

        [StringLength(128)]
        public string FulfillmentCenterName { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(16)]
        public string WeightUnit { get; set; }

        public decimal? WeightValue { get; set; }

        public decimal? VolumetricWeight { get; set; }

        [StringLength(16)]
        public string DimensionUnit { get; set; }

        public decimal? DimensionHeight { get; set; }

        public decimal? DimensionLength { get; set; }

        public decimal? DimensionWidth { get; set; }

        public bool TaxIncluded { get; set; }

        [Column(TypeName = "Money")]
        public decimal Price { get; set; }

        [Column(TypeName = "Money")]
        public decimal PriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        public decimal TaxPercentRate { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }

        [Column(TypeName = "Money")]
        public decimal TotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal Fee { get; set; }

        [Column(TypeName = "Money")]
        public decimal FeeWithTax { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public virtual ObservableCollection<ShipmentItemEntity> Items { get; set; } = new NullCollection<ShipmentItemEntity>();
        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();


        public virtual Shipment ToModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            shipment.InjectFrom(this);
            // workaround for mis-spelling 
            shipment.FulfillmentCenterId = FulfilmentCenterId;

            if (!Addresses.IsNullOrEmpty())
            {
                shipment.DeliveryAddress = Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }

            if (!Discounts.IsNullOrEmpty())
            {
                shipment.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }

            if (!Items.IsNullOrEmpty())
            {
                shipment.Items = Items.Select(x => x.ToModel(AbstractTypeFactory<ShipmentItem>.TryCreateInstance())).ToList();
            }

            if (!TaxDetails.IsNullOrEmpty())
            {
                shipment.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            return shipment;
        }

        public virtual ShipmentEntity FromModel(Shipment shipment, PrimaryKeyResolvingMap pkMap)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            pkMap.AddPair(shipment, this);

            this.InjectFrom(shipment);
            // workaround for mis-spelling 
            FulfilmentCenterId = shipment.FulfillmentCenterId;

            //Allow to empty address
            Addresses = new ObservableCollection<AddressEntity>();
            if (shipment.DeliveryAddress != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(new[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(shipment.DeliveryAddress) });
            }
            if (shipment.Items != null)
            {
                Items = new ObservableCollection<ShipmentItemEntity>(shipment.Items.Select(x => AbstractTypeFactory<ShipmentItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
                foreach (var shipmentItem in Items)
                {
                    shipmentItem.ShipmentId = Id;
                }
            }

            if (shipment.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(shipment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (shipment.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(shipment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            return this;
        }

        public virtual void Patch(ShipmentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Fee = Fee;
            target.FeeWithTax = FeeWithTax;
            target.FulfilmentCenterId = FulfilmentCenterId;
            target.FulfillmentCenterName = FulfillmentCenterName;
            target.ShipmentMethodCode = ShipmentMethodCode;
            target.Total = Total;
            target.TotalWithTax = TotalWithTax;
            target.TaxTotal = TaxTotal;
            target.Price = Price;
            target.PriceWithTax = PriceWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.TaxPercentRate = TaxPercentRate;
            target.TaxIncluded = TaxIncluded;
            target.Currency = Currency;
            target.WeightUnit = WeightUnit;
            target.WeightValue = WeightValue;
            target.DimensionHeight = DimensionHeight;
            target.DimensionLength = DimensionLength;
            target.DimensionUnit = DimensionUnit;
            target.DimensionWidth = DimensionWidth;
            target.TaxType = TaxType;
            target.ShipmentMethodOption = ShipmentMethodOption;

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AbstractTypeFactory<TaxDetailEntityComparer>.TryCreateInstance();
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AbstractTypeFactory<DiscountEntityComparer>.TryCreateInstance();
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }
        }
    }
}
